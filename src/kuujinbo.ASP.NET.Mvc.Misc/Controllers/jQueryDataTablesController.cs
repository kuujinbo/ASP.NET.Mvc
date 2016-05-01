using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using kuujinbo.ASP.NET.Mvc.Misc.Models;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    public class jQueryDataTablesController : Controller
    {
        private Table InitDataTable(UrlHelper url)
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton(url.Action("Create"), "Create")
                    { 
                        IsButton = false
                    },
                    new ActionButton(url.Action("Rollover"), "Rollover")
                    { 
                        CssClass = ActionButton.Primary,
                    },
                    new ActionButton(url.Action("Approve"), "Approve"),
                    new ActionButton(url.Action("Disapprove"), "Disapprove")
                    { 
                        CssClass = ActionButton.Danger,
                    }
                },
                DataUrl = url.Action("JsonData"),
                DeleteRowUrl = url.Action("DeleteOne"),
                EditRowUrl = url.Action("Update"),
            };
            table.SetColumns<TestModel>();

            return table;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "jQuery DataTables Test";
            var table = InitDataTable(Url);
            return View("_jQueryDataTables", table);
        }

        [HttpAjaxPost]
        public ActionResult JsonData(Table table)
        {
            System.Diagnostics.Debug.WriteLine(
                JsonNetSerializer.Get(Request.Form)
            );

            string dataFile = Server.MapPath("~/app_data/dataTablesObjectData.json");
            string json = System.IO.File.ReadAllText(dataFile);
            var dataFromFile = JsonConvert.DeserializeObject<IEnumerable<TestModel>>(json);

            //System.Threading.Thread.Sleep(1000);
            //throw new Exception("error");
            return new JsonNetResult(table.GetData<TestModel>(dataFromFile));
        }

        private object GetBatchUpdateResponseObject(IEnumerable<int> ids)
        {
            return string.Format(
                "XHR sent to:{2}[{0}]{2}with POST data [{1}]{2}succeeded!",
                Request.Url, string.Join(", ",  ids), Environment.NewLine
            );
        }

        [HttpAjaxPost]
        public ActionResult Approve(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        [HttpAjaxPost]
        public ActionResult Disapprove(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        [HttpAjaxPost]
        public ActionResult Rollover(IEnumerable<int> ids)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(ids));
        }

        [HttpAjaxPost]
        public ActionResult DeleteOne(int id)
        {
            return new JsonNetResult(GetBatchUpdateResponseObject(new int[] {id}));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int id)
        {
            System.Threading.Thread.Sleep(2000);
            return new JsonNetResult(
                string.Format(
                    "XHR POST sent to [{0}] succeeded!", Request.Url
                )
            );
        }

        public ActionResult Update(int id)
        {
            return View(id);
        }


    }
}