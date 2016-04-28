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
    public class TableTests
    {

        [Fact]
        public void ExecuteResult_WithNullData_ThrowsArgumentNullException()
        {
        }

    }

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
            // var buttons = xElement.Elements("button").ToList();

            Assert.Equal(expectedCount, xElement.Nodes().Count());
            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }


    }
}