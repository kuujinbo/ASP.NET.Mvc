﻿using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests.Services.JqueryDataTables
{
    public class DataTableModelBinderTests
    {
        DataTableModelBinder _binder;
        NameValueCollection _form;
        Mock<HttpContextBase> _mockContext;
        ControllerContext _controllerContext;
        Mock<ControllerBase> _mockController;
        ModelBindingContext _modelBindingContext;
        Table _table;

        public DataTableModelBinderTests()
        {
            Setup();
        }

        void Setup()
        {
            _mockController = new Mock<ControllerBase>();
            _form = new NameValueCollection();

            var mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(r => r.Form).Returns(_form);

            _mockContext = new Mock<HttpContextBase>();
            _mockContext.Setup(c => c.Request).Returns(mockRequest.Object);

            var requestContext = new RequestContext(
                _mockContext.Object, new RouteData()
            );
            _controllerContext = new ControllerContext(
                requestContext, _mockController.Object
            );
            _binder = new DataTableModelBinder();

            _modelBindingContext = new ModelBindingContext
            {
                ValueProvider = new NameValueCollectionValueProvider(
                    _form, CultureInfo.CurrentCulture
                ),
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(
                    null, typeof(Table)
                )
            };
        }

/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * jQuery DataTables sends HTTP request form parameters in this **EXACT**
 * format. 'regex' form values are for reference only - don't and will
 * __NEVER__ implement Regex search
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! */
        void FakeHttpPostVisibleCheckColumn()
        {
            /* ================================================================
             * first column **ALWAYS** sent, irregardless of UI visiblity
             * ================================================================
             */
            _form["columns[0][data]"] = "0";
            _form["columns[0][name]"] = "";
            _form["columns[0][searchable]"] = "false";
            _form["columns[0][orderable]"] = "false";
            _form["columns[0][search][value]"] = "";
            _form["columns[0][search][regex]"] = "false";

            _form["draw"] = "0";
            _form["start"] = "0";
            _form["length"] = "10";
            _form["checkColumn"] = "true";
            /* ================================================================
             * actual per column data processed starts here
             * ================================================================
             */
            _form["columns[1][data]"] = "1";
            _form["columns[1][name]"] = "";
            _form["columns[1][searchable]"] = "true";
            _form["columns[1][orderable]"] = "true";
            _form["columns[1][search][regex]"] = "false";
            _form["columns[2][data]"] = "2";
            _form["columns[2][name]"] = "";
            _form["columns[2][searchable]"] = "true";
            _form["columns[2][orderable]"] = "true";
            _form["columns[2][search][regex]"] = "false";
            /* ================================================================
             * last column - per row actions 
             * ================================================================
             */
            _form["columns[3][data]"] = "3";
            _form["columns[3][name]"] = "";
            _form["columns[3][searchable]"] = "false";
            _form["columns[3][orderable]"] = "false";
            _form["columns[3][search][value]"] = "";
            _form["columns[3][search][regex]"] = "false";

            // multi-column sort
            _form["order[0][column]"] = "1";
            _form["order[0][dir]"] = "desc";
            _form["order[1][column]"] = "2";
            _form["order[1][dir]"] = "desc";

            // multi-column search; the model binder accounts for
            // jQuery DataTables column index
            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
             * NOTE: checkbox column visibility determines column index:
             * see same section in FakeHttpPostHiddenCheckColumn()
             * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! */
            _form["columns[1][search][value]"] = "SEARCH_00";
            _form["columns[2][search][value]"] = "SEARCH_01";

            _form["search[value]"] = "global search value";
            _form["search[regex]"] = "false";
        }

        void FakeHttpPostHiddenCheckColumn()
        {
            _form["columns[0][data]"] = "0";
            _form["columns[0][name]"] = "";
            _form["columns[0][searchable]"] = "true";
            _form["columns[0][orderable]"] = "true";
            _form["columns[0][search][regex]"] = "false";

            _form["draw"] = "0";
            _form["start"] = "0";
            _form["length"] = "10";
            _form["checkColumn"] = "false";

            _form["columns[1][data]"] = "1";
            _form["columns[1][name]"] = "";
            _form["columns[1][searchable]"] = "true";
            _form["columns[1][orderable]"] = "true";
            _form["columns[1][search][regex]"] = "false";
            _form["columns[2][data]"] = "2";
            _form["columns[2][name]"] = "";
            _form["columns[2][searchable]"] = "true";
            _form["columns[2][orderable]"] = "true";
            _form["columns[2][search][value]"] = "";
            _form["columns[2][search][regex]"] = "false";

            _form["columns[3][data]"] = "3";
            _form["columns[3][name]"] = "";
            _form["columns[3][searchable]"] = "false";
            _form["columns[3][orderable]"] = "false";
            _form["columns[3][search][value]"] = "";
            _form["columns[3][search][regex]"] = "false";

            _form["order[0][column]"] = "1";
            _form["order[0][dir]"] = "desc";
            _form["order[1][column]"] = "2";
            _form["order[1][dir]"] = "asc";

            _form["columns[0][search][value]"] = "SEARCH_00";
            _form["columns[1][search][value]"] = "SEARCH_01";

            _form["search[value]"] = "global search value";
            _form["search[regex]"] = "false";
        }

        [Fact]
        public void BindModel_VisibleCheckboxColumn_MapsFormCollectionToModel()
        {
            FakeHttpPostVisibleCheckColumn();

            _table = _binder.BindModel(_controllerContext, _modelBindingContext) as Table;
            Assert.NotNull(_table);
            Assert.Equal(3, _table.Columns.Count());
            Assert.Equal<int>(0, _table.Draw);
            Assert.Equal<int>(0, _table.Start);
            Assert.Equal<int>(10, _table.Length);
            Assert.Equal<bool>(true, _table.CheckboxColumn);
            Assert.Equal<string>("global search value", _table.Search.Value);

            Column data0 = _table.Columns.ElementAt(0);
            Column data1 = _table.Columns.ElementAt(1);
            Column nonData = _table.Columns.ElementAt(2);
            Assert.Equal<string>("1", data0.Data);
            Assert.Equal<string>("2", data1.Data);
            Assert.Equal<string>("3", nonData.Data);
            Assert.Equal<bool>(true, data0.IsSearchable);
            Assert.Equal<bool>(true, data1.IsSearchable);
            Assert.Equal<bool>(false, nonData.IsSearchable);
            Assert.Equal<bool>(true, data0.IsSortable);
            Assert.Equal<bool>(true, data1.IsSortable);
            Assert.Equal<bool>(false, nonData.IsSortable);

            Assert.Equal<int>(0, _table.SortOrders.ElementAt(0).ColumnIndex);
            Assert.Equal<int>(1, _table.SortOrders.ElementAt(1).ColumnIndex);
            Assert.Equal<string>("desc", _table.SortOrders.ElementAt(0).Direction);
            Assert.Equal<string>("desc", _table.SortOrders.ElementAt(0).Direction);

            Assert.Equal<int>(0, data0.Search.ColumnIndex);
            Assert.Equal<int>(1, data1.Search.ColumnIndex);

            Assert.Equal<string>("SEARCH_00", data0.Search.Value);
            Assert.Equal<string>("SEARCH_01", data1.Search.Value);
        }

        [Fact]
        public void BindModel_HiddenCheckboxColumn_MapsFormCollectionToModel()
        {
            FakeHttpPostHiddenCheckColumn();

            _table = _binder.BindModel(_controllerContext, _modelBindingContext) as Table;
            Assert.NotNull(_table);
            Assert.Equal(3, _table.Columns.Count());
            Assert.Equal<int>(0, _table.Draw);
            Assert.Equal<int>(0, _table.Start);
            Assert.Equal<int>(10, _table.Length);
            Assert.Equal<bool>(false, _table.CheckboxColumn);
            Assert.Equal<string>("global search value", _table.Search.Value);

            Column data0 = _table.Columns.ElementAt(0);
            Column data1 = _table.Columns.ElementAt(1);
            Column nonData = _table.Columns.ElementAt(2);

            Assert.Equal<string>("1", data0.Data);
            Assert.Equal<string>("2", data1.Data);
            Assert.Equal<string>("3", nonData.Data);
            Assert.Equal<bool>(true, data0.IsSearchable);
            Assert.Equal<bool>(true, data1.IsSearchable);
            Assert.Equal<bool>(false, nonData.IsSearchable);
            Assert.Equal<bool>(true, data0.IsSortable);
            Assert.Equal<bool>(true, data1.IsSortable);
            Assert.Equal<bool>(false, nonData.IsSortable);

            Assert.Equal<int>(0, _table.SortOrders.ElementAt(0).ColumnIndex);
            Assert.Equal<int>(1, _table.SortOrders.ElementAt(1).ColumnIndex);
            Assert.Equal<string>("desc", _table.SortOrders.ElementAt(0).Direction);
            Assert.Equal<string>("desc", _table.SortOrders.ElementAt(0).Direction);

            Assert.Equal<int>(0, data0.Search.ColumnIndex);
            Assert.Equal<int>(1, data1.Search.ColumnIndex);

            Assert.Equal<string>("SEARCH_00", data0.Search.Value);
            Assert.Equal<string>("SEARCH_01", data1.Search.Value);
        }
    }
}