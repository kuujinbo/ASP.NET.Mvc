using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using kuujinbo.Mvc.NET.Examples.Models;
using Newtonsoft.Json;

namespace kuujinbo.Mvc.NET.Examples.Controllers
{
    [ExcludeFromCodeCoverage]
    public class JQueryAutoCompleteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchUsers(string searchText)
        {
            var users = from user in ModelData.Data
                        where user.Name.StartsWith(
                            searchText, StringComparison.OrdinalIgnoreCase
                        )
                        select user;

            var result = new List<dynamic>();
            foreach (var user in users)
            {
                result.Add(new
                {
                    label = user.Name
                    ,
                    value = user.Id
                    ,
                    office = user.Office
                });
            }

            System.Threading.Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpPost]
        public ActionResult AddUsers(string[] users)
        {
            return new JsonResult
            {
                Data = string.Format(
                    "<h1>Users Added</h1> {0}",
                    string.Join("<br />", users)
                )
            };
        }
    }
}