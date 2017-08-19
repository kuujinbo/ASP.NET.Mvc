using System.IO;
using System.Web;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Models;

namespace kuujinbo.ASP.NET.Mvc.Examples.Controllers
{
    public class FluentTestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(TestModel model, HttpPostedFileBase simpleFileUpload)
        {
            if (ModelState.IsValid)
            {
                if (simpleFileUpload != null 
                    && simpleFileUpload.ContentLength > 0)
                {
                    simpleFileUpload.SaveAs(Path.Combine(
                        Server.MapPath("~/app_data"),
                        Path.GetFileName(simpleFileUpload.FileName)
                    ));
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
            /*
             * #############################################################
             * if fluentvalidation.mvc not used/registered
             * #############################################################
            TestModelValidator validator = new TestModelValidator();
            var result = validator.Validate(model);
            if (result.IsValid)
            if (ModelState.IsValid) 
            {
                return View(model);
            }
            else {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("Index", model); 
            }
             */
        }
    }
}