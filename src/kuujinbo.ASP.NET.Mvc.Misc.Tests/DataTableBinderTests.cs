using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Misc.Tests
{
    public class DataTableBinderTests
    {
        DataTableModelBinder _binder;
        NameValueCollection _form;
        Mock<HttpContextBase> _mockContext;
        ControllerContext _controllerContext;
        Mock<ControllerBase> _mockController;
        ModelBindingContext _modelBindingContext;
        Table _table;

        void Setup()
        {
            _mockController = new Mock<ControllerBase>();
            _form = new NameValueCollection();
            SetFormCollection();

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
        void SetFormCollection()
        {
            _form["draw"] = "1";
            _form["start"] = "1";
            _form["length"] = "10";
            _form["columns[0][name]"] = "columns[0][name]";
            _form["columns[1][name]"] = "columns[1][name]";
            _form["columns[0][data]"] = "8";
            _form["columns[1][data]"] = "20";
            _form["columns[0][searchable]"] = "false";
            _form["columns[1][searchable]"] = "true";
            _form["columns[0][search][value]"] = "column 1 search value";
            _form["columns[1][search][value]"] = "column 2 search value";
            _form["columns[0][orderable]"] = "false";
            _form["columns[1][orderable]"] = "True";
            _form["order[0][column]"] = "0";
            _form["order[0][dir]"] = "asc";
            _form["order[1][column]"] = "1";
            _form["order[1][dir]"] = "desc";
            _form["search[value]"] = "global search value";
        }

        public DataTableBinderTests()
        {
            Setup();
        }

        [Fact]
        public void BindModel_WithFormData_CreatesTheTable()
        {
            _table = _binder.BindModel(_controllerContext, _modelBindingContext) as Table;
            Column c0 = _table.Columns.ElementAt(0);
            Column c1 = _table.Columns.ElementAt(1);

            Assert.NotNull(_table);
            Assert.Equal<int>(1, _table.Draw);
            Assert.Equal<int>(1, _table.Start);
            Assert.Equal<int>(10, _table.Length);
            Assert.Equal(2, _table.Columns.Count());
            Assert.Equal<string>("8", c0.Data);
            Assert.Equal<string>("20", c1.Data);
            Assert.Equal<bool>(false, c0.IsSearchable);
            Assert.Equal<bool>(true, c1.IsSearchable);
            Assert.Equal<bool>(false, c0.IsSortable);
            Assert.Equal<bool>(true, c1.IsSortable);
            Assert.Null(c0.Search); // c0 is **NOT** searchable
            Assert.Equal<string>("column 2 search value", c1.Search.Value);
            Assert.Equal<int>(0, _table.SortOrders.ElementAt(0).Column);
            Assert.Equal<int>(1, _table.SortOrders.ElementAt(1).Column);
            Assert.Equal<string>("asc", _table.SortOrders.ElementAt(0).Direction);
            Assert.Equal<string>("desc", _table.SortOrders.ElementAt(1).Direction);
            Assert.Equal<string>("global search value", _table.Search.Value);
        }
    }
}