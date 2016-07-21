using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using Xunit;
using Xunit.Abstractions;

namespace kuujinbo.ASP.NET.Mvc.Tests.Services.JqueryDataTables
{
    /* --------------------------------------------------------------------
     * HTML/JavaScript written to Partial View
     * --------------------------------------------------------------------
     */
    public class TableHtmlWriterTests
    {
        private readonly ITestOutputHelper _output;

        public TableHtmlWriterTests(ITestOutputHelper output)
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
         * writing the table thead and tfoot (TableHtmlWriter.cs partial class)
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
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
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
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void GetGetTableHtml_WithColumnDisplayWidth_AddsInlineStyleToThead()
        {
            var columns = new List<Column>()
            { 
                new Column() { Display = true } ,
                new Column() { Display = true, DisplayWidth = 20 } 
            };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            var theads = xElement.XPathSelectElements("//thead/tr/th");

            // first column is checkbox, last for per-row actions
            Assert.Equal(4, theads.Count());
            // first **data** column should not have inline style: DisplayWidth == 0
            Assert.Null(theads.ElementAt(1).Attribute("style"));

            Assert.Equal(
                string.Format("width:{0}%", columns[1].DisplayWidth),
                theads.ElementAt(2).Attribute("style").Value
            );
        }

        [Fact]
        public void GetGetTableHtml_WithABoolPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>() { new Column() { Type = typeof(bool) } };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(
                "", expectedDataSet.Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();

            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal("true", options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("Yes", options.ElementAt(1).Value);
            Assert.Equal("false", options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("No", options.ElementAt(2).Value);
        }

        [Fact]
        public void GetGetTableHtml_WithNullableBoolPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>()
            { 
                new Column() { Type = typeof(bool?), IsSearchable = true } 
            };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");
            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();

            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal("true", options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("Yes", options.ElementAt(1).Value);
            Assert.Equal("false", options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("No", options.ElementAt(2).Value);
        }

        public enum TestEnum { OneTwo, ThreeFour }
        [Fact]
        public void GetGetTableHtml_WithEnumPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>()
            { 
                new Column() { Type = typeof(TestEnum) },
                new Column() { Type = typeof(TestEnum), IsSearchable = true } 
            };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            var expectedDataSet = xElement.XPathSelectElements("//th[@data-is-searchable]");
            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(table.Columns.ElementAt(1).IsSearchable, true);
            Assert.Equal(2, xElement.Nodes().Count());
            Assert.Equal(
                "", expectedDataSet.ElementAt(0).Attribute("data-is-searchable").Value
            );
            Assert.Equal(
                "true", expectedDataSet.ElementAt(1).Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal(TestEnum.OneTwo.ToString(), options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("One Two", options.ElementAt(1).Value);
            Assert.Equal(TestEnum.ThreeFour.ToString(), options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("Three Four", options.ElementAt(2).Value);
        }

        [Fact]
        public void GetGetTableHtml_WithAnyOtherPropertyType_AddsTextInputFiltersToTfoot()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            var input = xElement.XPathSelectElement("//input[@type='text']");

            Assert.Equal("", input.Value);
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("style").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("data-column-number").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("placeholder").Value));
        }

        [Fact]
        public void GetGetTableHtml_LastTh_AddsSpansToTfoot()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.GetTableHtml()
            ));

            var lastTh = xElement.XPathSelectElement("//tfoot/tr/th[last()]");
            var spans = lastTh.Nodes().OfType<XElement>();
            var spanCount = 3;

            Assert.False(string.IsNullOrWhiteSpace(lastTh.Attribute("style").Value));
            Assert.Equal(spanCount, spans.Count());
            for (int i = 0; i < spanCount; ++i)
            {
                Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(i).Attribute("class").Value));
                Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(i).Attribute("title").Value));
                Assert.Equal("", spans.ElementAt(i).Value);
            }
            Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(spanCount - 1).Attribute("id").Value));
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

            Assert.Equal<int>(6, lines.Count());
            Assert.StartsWith("{", json);
            Assert.Equal<int>(dataUrl.Count(x => x == '"'), 4);
            Assert.Matches("^\"dataUrl\"", dataUrl);
            Assert.Equal<int>(dataUrl.Count(x => x == ':'), 1);
            Assert.Equal<int>(dataUrl.Count(x => x == '/'), 1);
            Assert.EndsWith("}", json);
        }

        [Fact]
        public void GetJavaScriptConfig_WhenSaveAsTrue_AddsColumnNames()
        {
            var table = new Table()
            {
                SaveAs = true,
                DataUrl = "/"
            };
            table.SetColumns<TestModel>();

            var json = table.GetJavaScriptConfig();

            Assert.Contains(DataTableModelBinder.COLUMN_NAMES, json);
            Assert.Equal(5, table.ColumnNames.Length);
            Assert.Contains(table.ColumnNames[0], json);
            Assert.Contains(table.ColumnNames[1], json);
            Assert.Contains(table.ColumnNames[2], json);
            Assert.Contains(table.ColumnNames[3], json);
            Assert.Contains(table.ColumnNames[4], json);
        }


        [Fact]
        public void GetScriptElements_WhenScriptPathsNull_ReturnsStringEmpty()
        {
            var table = new Table();

            Assert.Equal(string.Empty, table.GetScriptElements());
        }

        [Fact]
        public void GetScriptElements_WhenScriptPathsEmptyReturnsStringEmpty()
        {
            var table = new Table() {ScriptPaths = new string[] {} };

            Assert.Equal(string.Empty, table.GetScriptElements());
        }

        [Fact]
        public void GetScriptElements_WhenScriptPathsNotEmptyReturnsScriptTags()
        {
            var scripts = new string[] { "0.js", "1.js", "2.js", "3.js", "4.js"};
            var table = new Table() { ScriptPaths = scripts };

            var result = table.GetScriptElements().Split(
                new string[] {"\n"},
                StringSplitOptions.RemoveEmptyEntries
            );

            Assert.Equal(scripts.Length, result.Length);
            for (int i = 0; i < scripts.Length; ++i) 
            {
                var xElement = XElement.Parse(result[i]);

                Assert.Equal("script", xElement.Name.ToString());
                Assert.Equal(scripts[i], xElement.Attribute("src").Value);
                Assert.Equal(string.Empty, xElement.Value);
             }
        }
    }
}