using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services.JqueryDataTables
{
    /* --------------------------------------------------------------------
     * test model
     * --------------------------------------------------------------------
     */
    public class TestModel : IIdentifiable
    {
        public TestModel()
        {
            Hobbies = new List<TestHobby>();
        }

        public int Id { get; set; }
        [DataTableColumn(DisplayOrder = 1)]
        public string Name { get; set; }
        [DataTableColumn(DisplayOrder = 2)]
        public string Office { get; set; }
        [DataTableColumn(DisplayOrder = 3, DisplayName = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataTableColumn(DisplayOrder = 4, FieldAccessor = "Amount")]
        public TestSalary Salary { get; set; }

        [DataTableColumn(DisplayOrder = 5, FieldAccessor = "Name")]
        public ICollection<TestHobby> Hobbies { get; set; }
    }
    public class TestSalary
    {
        public int Amount { get; set; }
    }
    public class TestHobby
    {
        public string Name { get; set; }
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
            StartDate = new DateTime(2008, 11, 28),
            Salary = new TestSalary() { Amount = 80000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "1"}, new TestHobby() { Name = "2"}
            }
        };
        public static readonly TestModel RAMOS = new TestModel
        {
            Id = 25,
            Name = "Ramos, Angelica",
            Office = "London",
            StartDate = new DateTime(2010, 1, 1),
            Salary = new TestSalary() { Amount = 70000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "3"}, new TestHobby() { Name = "4"}
            }
        };
        public static readonly TestModel GREER = new TestModel
        {
            Id = 20,
            Name = "Greer, Bradley",
            Office = "London",
            Salary = new TestSalary() { Amount = 50000 },
            Hobbies = new List<TestHobby>() 
            { 
                new TestHobby() { Name = "5"}
            }
        };

        public static IEnumerable<TestModel> GetModelData()
        {
            return new List<TestModel>() { SATO, RAMOS, GREER };
        }

        /// <summary>
        /// act and assert for current [Fact]
        /// </summary>
        /// <param name="entities">
        /// entity collection in **EXPECTED** order 
        /// </param>
        /// <remarks>perform arrange in **CURRENT** [Fact]</remarks>
        private void ActAndAssert(params TestModel[] entities)
        {
            // act
            dynamic result = _table.GetData<TestModel>(GetModelData());

            // assert
            int entityCount = entities.Length;
            for (int i = 0; i < entityCount; ++i)
            {
                Assert.Equal(entityCount, result.recordsTotal);
                Assert.Equal(entityCount, result.recordsFiltered);
                Assert.IsType<List<List<object>>>(result.data);

                Assert.Equal(entityCount, result.data.Count);

                Assert.Equal(entities[i].Name, result.data[i][0]);
                Assert.Equal(entities[i].Office, result.data[i][1]);
                Assert.Equal(entities[i].StartDate, result.data[i][2]);
                Assert.Equal(entities[i].Salary.Amount, result.data[i][3]);
                Assert.Equal(
                    string.Join(", ", entities[i].Hobbies.Select(x => x.Name)),
                    result.data[i][4]
                );
                Assert.Equal(entities[i].Id, result.data[i][5]);
            }
        }

        [Fact]
        public void SetColumns_WhenCalled_AddsColumnsToTable()
        {
            _table = new Table();

            _table.SetColumns<TestModel>();

            Assert.Equal(5, _table.Columns.Count());
            Assert.Equal("Name", _table.Columns.ElementAt(0).Name);
            Assert.Equal("Office", _table.Columns.ElementAt(1).Name);
            Assert.Equal("Start Date", _table.Columns.ElementAt(2).Name);
            Assert.Equal("Salary", _table.Columns.ElementAt(3).Name);
            Assert.Equal("Hobbies", _table.Columns.ElementAt(4).Name);
        }

        // no sort or search criteria
        [Fact]
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

            ActAndAssert(SATO, RAMOS, GREER);
        }

        [Fact]
        public void GetData_SortCriteriaNoSearchCriteria_ReturnsAscendingSort()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>() 
                { 
                    // sort ascending => 'Name' property
                    new SortOrder { Column = 0, Direction = DataTableModelBinder.ORDER_ASC } 
                }
            };
            _table.SetColumns<TestModel>();

            ActAndAssert(GREER, RAMOS, SATO);
        }

        [Fact]
        public void GetData_SortNonAscCriteriaNoSearchCriteria_ReturnsDescendingSort()
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

            ActAndAssert(SATO, RAMOS, GREER);
        }

        [Fact]
        public void GetData_SearchNullOrEmptyCriteria_IgnoresSearchAndReturnsAllData()
        {
            _table = new Table()
            {
                Draw = 1,
                Start = 0,
                Length = 10,
                SortOrders = new List<SortOrder>()
            };
            _table.SetColumns<TestModel>();

            _table.Columns.ElementAt(1).Search = new Search() { Value = "  " };
            _table.Columns.ElementAt(2).Search = new Search();

            ActAndAssert(SATO, RAMOS, GREER);
        }

        [Fact]
        public void GetData_SearchCriteriaNoSortCriteria_ReturnsSearchMatchInOriginalOrder()
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
            _table.Columns.ElementAt(0).Search = new Search() { Value = "g" };

            ActAndAssert(RAMOS, GREER);
        }

        [Fact]
        public void GetData_SortAndSearchCriteria_ReturnsSearchMatchInRequestedOrder()
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
            _table.Columns.ElementAt(1).Search = new Search() { Value = "lon" };

            ActAndAssert(GREER, RAMOS);
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

        /* ===================================================================
         * writing the table thead and tfoot
         * ===================================================================
         */
        [Fact]
        public void GetGetTableHtml_WithNullColumns_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTableHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void GetGetTableHtml_WithEmptyColumns_ThrowsArgumentNullException()
        {
            var table = new Table() { Columns = new List<Column>() };
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.GetTableHtml()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }


        [Fact]
        public void GetGetTableHtml_WhenIsSearchableFalse_AddsEmptyDataSetAttribute()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            // throw new Exception(xElement.ToString());
            var expectedCount = TF_AUTO_COLUMS + columns.Count;
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(expectedCount, xElement.Nodes().Count());
            Assert.Equal(
                "", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void GetGetTableHtml_WhenIsSearchableTrue_AddsDataSetAttributeValue()
        {
            var columns = new List<Column>() { new Column() { IsSearchable = true } };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));
            var expectedCount = TF_AUTO_COLUMS + columns.Count;
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

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

            Assert.Equal<int>(5, lines.Count());
            Assert.StartsWith("{", json);
            Assert.Equal<int>(dataUrl.Count(x => x == '"'), 4);
            Assert.Matches("^\"dataUrl\"", dataUrl);
            Assert.Equal<int>(dataUrl.Count(x => x == ':'), 1);
            Assert.Equal<int>(dataUrl.Count(x => x == '/'), 1);
            Assert.EndsWith("}", json);
        }
    }


}