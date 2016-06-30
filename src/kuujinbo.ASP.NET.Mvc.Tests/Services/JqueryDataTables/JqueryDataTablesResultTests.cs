using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using kuujinbo.ASP.NET.Mvc.Tests;
using Xunit;
using Moq;

namespace kuujinbo.ASP.NET.Mvc.Tests.Services.JqueryDataTables
{
    public class FakeController : Controller
    {
        public ActionResult GetResults(Table table)
        {
            return new JqueryDataTablesResult(table);
        }
    }


    public class JqueryDataTablesResultTests
    {
        private FakeController _fakeController;

        public JqueryDataTablesResultTests()
        {
            _fakeController = new FakeController();
        }

        [Fact]
        public void ExecuteResult_WithNullObjectData_ThrowsArgumentNullException()
        {
            _fakeController.SetFakeControllerContext();

            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController.GetResults((Table)null)
            );

            Assert.Equal<string>("table", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithNullContext_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .GetResults(new Table())
                    .ExecuteResult(_fakeController.ControllerContext)
            );

            Assert.Equal<string>("context", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WhenSaveAsFalse_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.GetResults(new Table());
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal("application/json", _fakeController.Response.ContentType);
        }

        [Fact]
        public void ExecuteResult_WhenSaveAsTrue_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.GetResults(new Table() 
            { 
                SaveAs = true,
                Data = new List<List<object>>()
                {
                    new List<object>() {1, 2, 3},
                    new List<object>() {4, 5, 6}
                }
            });
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal(JqueryDataTablesResult.CONTENT_TYPE, _fakeController.Response.ContentType);
        }


    }
}