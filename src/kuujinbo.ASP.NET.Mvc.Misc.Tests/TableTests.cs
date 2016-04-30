using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests
{
    /* --------------------------------------------------------------------
     * test model
     * --------------------------------------------------------------------
     */
    public class TestModel : IIdentifiable
    {
        [DataTableColumn(
            Display = false, DisplayOrder = 0,
            IsSearchable = false, IsSortable = false)
        ]
        public int Id { get; set; }
        [DataTableColumn(DisplayOrder = 1)]
        public string Name { get; set; }
        [DataTableColumn(DisplayOrder = 2)]
        public string Office { get; set; }
        [DataTableColumn(DisplayOrder = 3, DisplayName = "Start Date")]
        public DateTime? StartDate { get; set; }
    }


    /* --------------------------------------------------------------------
     * model data and DataTableColumnAttribute
     * --------------------------------------------------------------------
     */
    public class TableTests
    {
        Table _table;

        public static readonly TestModel SATO = new TestModel
        {
            Id = 1,
            Name = "Satou, Airi",
            Office = "Tokyo",
            StartDate = new DateTime(2008, 11, 28)
        };
        public static readonly TestModel RAMOS = new TestModel
        {
            Id = 25,
            Name = "Ramos, Angelica",
            Office = "London",
            StartDate = new DateTime(2010, 1, 1)
        };
        public static readonly TestModel GREER = new TestModel
        {
            Id = 20,
            Name = "Greer, Bradley",
            Office = "London"
        };

        public static IEnumerable<TestModel> GetModelData()
        {
            return new List<TestModel>() { SATO, RAMOS, GREER };
        }

        [Fact]
        public void SetColumns_WhenCalled_AddsColumnsToTable()
        {
            _table = new Table();

            _table.SetColumns<TestModel>();

            Assert.Equal(4, _table.Columns.Count());
            Assert.Equal("Id", _table.Columns.ElementAt(0).Name);
            Assert.False(_table.Columns.ElementAt(0).Display);
            Assert.False(_table.Columns.ElementAt(0).IsSearchable);
            Assert.False(_table.Columns.ElementAt(0).IsSortable);
            Assert.Equal("Name", _table.Columns.ElementAt(1).Name);
            Assert.Equal("Office", _table.Columns.ElementAt(2).Name);
            Assert.Equal("Start Date", _table.Columns.ElementAt(3).Name);
        }

        [Fact]    // no sort or search criteria
        public void GetData_DefaultCall_ReturnsModelWithSpecifiedProperties()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();

            dynamic result = _table.GetData<TestModel>(GetModelData());

            Assert.Equal(3, result.recordsTotal);
            Assert.Equal(3, result.recordsFiltered);
            Assert.IsType<List<List<object>>>(result.data);
            Assert.Equal(3, result.data.Count);
            Assert.Equal(SATO.Id, result.data[0][0]);
            Assert.Equal(SATO.Name, result.data[0][1]);
            Assert.Equal(SATO.Office, result.data[0][2]);
            Assert.Equal(SATO.StartDate, result.data[0][3]);
            Assert.Equal(RAMOS.Id, result.data[1][0]);
            Assert.Equal(RAMOS.Name, result.data[1][1]);
            Assert.Equal(RAMOS.Office, result.data[1][2]);
            Assert.Equal(RAMOS.StartDate, result.data[1][3]);
            Assert.Equal(GREER.Id, result.data[2][0]);
            Assert.Equal(GREER.Name, result.data[2][1]);
            Assert.Equal(GREER.Office, result.data[2][2]);
            Assert.Equal(GREER.StartDate, result.data[2][3]);
            Assert.Null(result.data[2][3]);
        }

        /* -------------------------------------------------------------------
         * only sort, no search criteria. we only care about correct order,
         * not all the model properties - verified in preceding test.
         * -------------------------------------------------------------------
         */
        [Fact]
        public void GetData_WhenCalledWithSortCriteria_ReturnsSortedModelCollection()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Name' property
                    new SortOrder { Column = 1, Direction = DataTableModelBinder.ORDER_ASC } 
                }
            };
            _table.SetColumns<TestModel>();

            dynamic result = _table.GetData<TestModel>(GetModelData());

            Assert.Equal(3, result.recordsTotal);
            Assert.Equal(3, result.recordsFiltered);
            Assert.Equal(GREER.Name, result.data[0][1]);
            Assert.Equal(RAMOS.Name, result.data[1][1]);
            Assert.Equal(SATO.Name, result.data[2][1]);
        }

        /* -------------------------------------------------------------------
         * only sort, no search criteria. we only care about correct order,
         * not all the model properties.
         * -------------------------------------------------------------------
         */
        [Fact]
        public void GetData_WhenCalledWithSortNonAscCriteria_ReturnsDescendingSortModelCollection()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Name' property
                    // anything other than DataTableModelBinder.ORDER_ASC is descnding 
                    new SortOrder { Column = 1, Direction = "anything other than 'asc'" } 
                }
            };
            _table.SetColumns<TestModel>();

            dynamic result = _table.GetData<TestModel>(GetModelData());

            Assert.Equal(3, result.recordsTotal);
            Assert.Equal(3, result.recordsFiltered);
            Assert.Equal(SATO.Name, result.data[0][1]);
            Assert.Equal(RAMOS.Name, result.data[1][1]);
            Assert.Equal(GREER.Name, result.data[2][1]);
        }

        /* -------------------------------------------------------------------
         * only search, no sort criteria. we only care that the correct number
         * of records and model instances are returned.
         * -------------------------------------------------------------------
         */
        [Fact]
        public void GetData_WhenCalledWithSearchCriteria_ReturnsCorrectModelCollection()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();
            // search 'Name' property => case-insensitive
            _table.Columns.ElementAt(1).Search = new Search() { Value = "g" };

            dynamic result = _table.GetData<TestModel>(GetModelData());

            Assert.Equal(2, result.recordsTotal);
            Assert.Equal(2, result.recordsFiltered);
            Assert.Equal(RAMOS.Name, result.data[0][1]);
            Assert.Equal(GREER.Name, result.data[1][1]);
        }

        /* -------------------------------------------------------------------
         * sort **AND** search criteria. we only care that the correct number
         * of records and model instances are returned.
         * -------------------------------------------------------------------
         */
        [Fact]
        public void GetData_WhenCalledWithSortAndSearchCriteria_ReturnsCorrectModelCollection()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'StartDate' property
                    new SortOrder { Column = 3, Direction = DataTableModelBinder.ORDER_ASC },
                    /*
                     * sort descending => 'Name' property, which should be
                     * ignored, since 'StartDate' is evaluated first
                     */
                    new SortOrder { Column = 1, Direction = "other" },
                }
            };
            _table.SetColumns<TestModel>();
            // search 'Office' property => case-insensitive
            _table.Columns.ElementAt(2).Search = new Search() { Value = "lon" };

            dynamic result = _table.GetData<TestModel>(GetModelData());

            Assert.Equal(2, result.recordsTotal);
            Assert.Equal(2, result.recordsFiltered);
            Assert.Equal(GREER.Name, result.data[0][1]);
            Assert.Equal(RAMOS.Name, result.data[1][1]);
        }
    }


    /* --------------------------------------------------------------------
     * HTML/JavaScript written to Partial View
     * --------------------------------------------------------------------
     */
    public class TableToPartialViewTests
    {
        const int TH_AUTO_COLUMS = 2;
        const int TF_AUTO_COLUMS = 1;
        private readonly ITestOutputHelper _output;

        public TableToPartialViewTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ActionButtonsHtml_WithoutActionButton_WritesEmptyString()
        {
            var table = new Table();

            var result = table.ActionButtonsHtml();

            Assert.Equal(0, table.ActionButtons.Count);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ActionButtonsHtml_WithActionButton_WritesHtml()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("/Create", "Create"),
                    new ActionButton("/Delete", "Delete")
                }
            };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.ActionButtonsHtml()
            ));

            Assert.Equal(2, table.ActionButtons.Count);
            Assert.Equal(2, xElement.Nodes().Count());
        }

        [Fact]
        public void GetTheadHtml_WithNullColumns_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTheadHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void GetTheadHtml_WithEmptyColumns_ThrowsArgumentNullException()
        {
            var table = new Table() { Columns = new List<Column>()};
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTheadHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void GetTheadHtml_WithColumnDisplayFalse_DoesNotAddTh()
        {
            var table = new Table { Columns = new List<Column> { new Column() } };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTheadHtml()
            ));

            Assert.Equal(table.Columns.ElementAt(0).Display, false);
            Assert.Equal(TH_AUTO_COLUMS, xElement.Nodes().Count());
        }

        [Fact]
        public void GetTheadHtml_WithColumnDisplayTrue_AddsTh()
        {
            var columns = new List<Column>() { new Column() {Display = true} };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTheadHtml()
            ));
            var expected = TH_AUTO_COLUMS + columns.Count;

            Assert.Equal(expected, xElement.Nodes().Count());
        }

        [Fact]
        public void GetTfootHtml_WithNullColumns_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTfootHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void GetTfootHtml_WithEmptyColumns_ThrowsArgumentNullException()
        {
            var table = new Table() { Columns = new List<Column>() };
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTfootHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void GetTfootHtml_WhenIsSearchableFalse_AddsEmptyDataSetAttribute()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTfootHtml()
            ));
            var expectedCount = TF_AUTO_COLUMS + columns.Count;
            var expectedDataSet = xElement.XPathSelectElement("th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(expectedCount, xElement.Nodes().Count());
            Assert.Equal(
                "", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void GetTfootHtml_WhenIsSearchableTrue_AddsDataSetAttributeValue()
        {
            var columns = new List<Column>() { new Column() {IsSearchable = true} };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTfootHtml()
            ));
            var expectedCount = TF_AUTO_COLUMS + columns.Count;
            var expectedDataSet = xElement.XPathSelectElement("th[@data-is-searchable]");

            Assert.Equal(expectedCount, xElement.Nodes().Count());
            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void GetJavaScriptConfig_WhenDataUrlIsNull_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => new Table().GetJavaScriptConfig()
            );

            Assert.Equal<string>("DataUrl", exception.ParamName);
        }

        [Fact]
        public void GetJavaScriptConfig_WhenDataUrlIsEmpty_ThrowsArgumentNullException()
        {
            var table = new Table() { DataUrl = string.Empty };

            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetJavaScriptConfig()
            );

            Assert.Equal<string>("DataUrl", exception.ParamName);
        }

        [Fact]
        public void GetJavaScriptConfig_WhenDataUrlNotWhiteSpace_ReturnsJsonWithDataUrl()
        {
            var table = new Table() { DataUrl = "/" };

            var json = table.GetJavaScriptConfig();
            var lines = json.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Where(x => x != "{" && x != "}");
            var dataUrl = lines.ElementAt(0).Trim();

            Assert.Equal<int>(4, lines.Count());
            Assert.StartsWith("{", json);
            Assert.Equal<int>(dataUrl.Count(x => x == '"'), 4);
            Assert.Matches("^\"dataUrl\"", dataUrl);
            Assert.Equal<int>(dataUrl.Count(x => x == ':'), 1);
            Assert.Equal<int>(dataUrl.Count(x => x == '/'), 1);
            Assert.EndsWith("}", json);
        }

    }
}