﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using kuujinbo.ASP.NET.Mvc.Misc.ViewModels;
using Newtonsoft.Json.Converters;

namespace kuujinbo.ASP.NET.Mvc.Misc.Controllers
{
    public class jQueryDataTablesController : Controller
    {
        private JqueryDataTable InitDataTable(UrlHelper url)
        {
            var table = new JqueryDataTable()
            {
                ActionButtons = new List<JqueryDataTable.ActionButton>()
                {
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Success,
                        Url = url.Action("Create"),
                        Text = "Create",
                        IsButton = false
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Primary,
                        Url = url.Action("Rollover"),
                        Text = "Rollover"
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Success,
                        Url = url.Action("Approve"),
                        Text = "Approve"
                    },
                    new JqueryDataTable.ActionButton 
                    { 
                        ElementClass = JqueryDataTable.ActionButton.Danger,
                        Url = url.Action("Disapprove"),
                        Text = "Disapprove"
                    }
                },
                DataUrl = url.Action("JsonData"),
                DeleteRowUrl = url.Action("DeleteOne"),
                EditRowUrl = url.Action("Update"),
                LastColumnIndex = 7,
            };
            table.SetColumns<TestModel>();

            return table;
        }

        // GET: jQueryDataTables
        public ActionResult Index()
        {
            var table = InitDataTable(Url);
            return View("_jQueryDataTables", table);
        }

        [HttpAjaxPost]
        // public ActionResult JsonData()
        public ActionResult JsonData(JqueryDataTable table)
        {
            string dataFile = Server.MapPath("~/app_data/dataTablesObjectData.json");
            string json = System.IO.File.ReadAllText(dataFile);
            var dataFromFile = JsonConvert.DeserializeObject<IEnumerable<TestModel>>(json);
            //System.Diagnostics.Debug.WriteLine(
            //    JsonNet.Serialize(table.GetData<TestModel>(dataFromFile))
            //);

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