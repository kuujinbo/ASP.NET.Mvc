using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using kuujinbo.ASP.NET.Mvc.Misc.ViewModels;
using kuujinbo.ASP.NET.Mvc.Misc.ViewModels.JqueryDataTables;
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
                    new ActionButton 
                    { 
                        ElementClass = ActionButton.Success,
                        Url = url.Action("Create"),
                        Text = "Create",
                        IsButton = false
                    },
                    new ActionButton 
                    { 
                        ElementClass = ActionButton.Primary,
                        Url = url.Action("Rollover"),
                        Text = "Rollover"
                    },
                    new ActionButton 
                    { 
                        ElementClass = ActionButton.Success,
                        Url = url.Action("Approve"),
                        Text = "Approve"
                    },
                    new ActionButton 
                    { 
                        ElementClass = ActionButton.Danger,
                        Url = url.Action("Disapprove"),
                        Text = "Disapprove"
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
            var table = InitDataTable(Url);
            return View("_jQueryDataTables", table);
        }

        [HttpAjaxPost]
        public ActionResult JsonData(Table table)
        {
            string dataFile = Server.MapPath("~/app_data/dataTablesObjectData.json");
            string json = System.IO.File.ReadAllText(dataFile);
            var dataFromFile = JsonConvert.DeserializeObject<IEnumerable<TestModel>>(json);

            //System.Diagnostics.Debug.WriteLine(
            //    JsonNet.Serialize(table.GetData<TestModel>(dataFromFile))
            //);
            // System.Threading.Thread.Sleep(2000);

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