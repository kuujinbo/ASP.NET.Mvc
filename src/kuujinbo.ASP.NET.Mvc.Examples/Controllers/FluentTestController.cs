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
        // public ActionResult Index(FormCollection model)
        public ActionResult Index(TestModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
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