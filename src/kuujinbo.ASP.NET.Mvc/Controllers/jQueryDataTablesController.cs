using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Threading;
using Newtonsoft.Json;
using kuujinbo.ASP.NET.Mvc.Services.Json;
using kuujinbo.ASP.NET.Mvc.Models;
using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;

namespace kuujinbo.ASP.NET.Mvc.Controllers
{
    public class jQueryDataTablesController : Controller
    {
        private static ICollection<TestModel> _data;

        /* ====================================================================
         * setup DataTable instance on initial HTTP request
         * ====================================================================
         */
        public ActionResult Index()
        {
            ViewBag.Title = "jQuery DataTables Test";
            var table = InitDataTable(Url);

            if (_data == null)
            {
                string dataFile = Server.MapPath("~/app_data/dataTablesObjectData00.json");
                _data = JsonConvert.DeserializeObject<ICollection<TestModel>>(
                    System.IO.File.ReadAllText(dataFile)
                );
                //int i = 0;
                //foreach (var d in _data)
                //{
                //    d.Salaried = ++i % 3 == 0 ? true : false;
                //}
                //System.IO.File.WriteAllText(
                //    Server.MapPath("~/app_data/dataTablesObjectData00.json"),
                //    JsonConvert.SerializeObject(_data, Formatting.Indented)
                //);
            }
            return View("_jQueryDataTables", table);
        }

        private Table InitDataTable(UrlHelper url)
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton(url.Action("Create"), "Create")
                    { 
                        BulkAction = false
                    }
                    ,
                    new ActionButton(url.Action("Index", "Reports"), "Reports")
                    { 
                        CssClass = ActionButton.Primary,
                        BulkAction = false,
                        Modal = true
                    },
                    new ActionButton(url.Action("Delete"), "Delete")
                    { 
                        CssClass = ActionButton.Danger,
                    }
                },
                DataUrl = url.Action("GetResults"),
                InfoRowUrl = url.Action("Info"),
                EditRowUrl = url.Action("Update"),
                DeleteRowUrl = url.Action("DeleteOne")
            };
            table.SetColumns<TestModel>();

            return table;
        }

        /* ====================================================================
         * all subsequent HTTP requests are done via XHR to update DataTable
         * ====================================================================
         */
        [HttpPost]
        public ActionResult GetResults(Table table)
        {
            Thread.Sleep(760);

            table.ExecuteRequest<TestModel>(_data);
            return new JqueryDataTablesResult(table);
        }

        /* ====================================================================
         * 'bulk' actions
         * ====================================================================
         */
        [HttpAjaxPost]
        public ActionResult Delete(IEnumerable<int> ids)
        {
            Thread.Sleep(760);
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        private object GetBatchUpdateResponseObject(IEnumerable<int> ids)
        {
            return string.Format(
                "XHR sent to:{2}[{0}]{2}with POST data [{1}]{2}succeeded!",
                Request.Url, string.Join(", ", ids), Environment.NewLine
            );
        }

        /* ====================================================================
         * hyperlink action buttons
         * ====================================================================
         */
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int id)
        {
            Thread.Sleep(760);
            return new JsonNetResult(
                string.Format(
                    "XHR POST sent to [{0}] succeeded!", Request.Url
                )
            );
        }

        /* ====================================================================
         * per-row/record actions
         * ====================================================================
         */
        public ActionResult Info(int id)
        {
            return View(id);
        }

        public ActionResult Update(int id)
        {
            return View(id);
        }

        [HttpAjaxPost]
        public ActionResult DeleteOne(int id)
        {
            Thread.Sleep(760);
            //var toDelete = _data.SingleOrDefault(x => x.Id == id);
            //if (toDelete != null)
            //{
            //    _data.Remove(toDelete);
            //    return new JsonNetResult(GetBatchUpdateResponseObject(new int[] { id }));
            //}

            return new HttpStatusCodeResult(
                HttpStatusCode.BadRequest,
                "There was a problem deleting the record. Please try again."
            );
        }
    }
}