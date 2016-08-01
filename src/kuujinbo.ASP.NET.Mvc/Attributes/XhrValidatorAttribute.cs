using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;

namespace System.Web.Mvc
{
    [ExcludeFromCodeCoverage]
    public sealed class XhrValidatorAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var modelState = filterContext.Controller.ViewData.ModelState;
                if (!modelState.IsValid)
                {
                    var errors = modelState.Keys
                        .Where(x => modelState[x].Errors.Count > 0)
                        .Select(e => new
                        {
                            name = e,
                            errorMessage = modelState[e]
                                .Errors.Select(y => y.ErrorMessage).ToArray()
                        });
                    filterContext.Result = new JsonResult() { Data = errors };
                    filterContext.HttpContext.Response
                        .StatusCode = (int) HttpStatusCode.BadRequest;
                    //                   400 Bad Request - ^^^^^^^^^^
                    // http://tools.ietf.org/html/rfc7231#section-6.5.1
                }  
            }
        }

        // not needed
        public void OnActionExecuting(ActionExecutingContext filterContext) { }
    }
}