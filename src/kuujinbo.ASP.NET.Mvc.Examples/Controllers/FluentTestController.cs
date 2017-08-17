using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Mvc;
using FluentValidation.Results;
using kuujinbo.ASP.NET.Mvc.Examples.Services;
using kuujinbo.ASP.NET.Mvc.Models;

namespace kuujinbo.ASP.NET.Mvc.Examples.Controllers
{
    public class FluentTestController : Controller
    {
        // GET: FluentTest
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        // public ActionResult Index(FormCollection model)
        public ActionResult Index(TestModel model)
        {
            TestModelValidator validator = new TestModelValidator();
            var result = validator.Validate(model);

            if (result.IsValid)
            {
                return View(model);
            }
            else {
                // result.AddToModelState(ModelState, null);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model); 
            }
        }
    }
}