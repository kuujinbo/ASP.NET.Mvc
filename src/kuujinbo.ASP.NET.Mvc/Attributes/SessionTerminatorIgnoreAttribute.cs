using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Attributes
{
    public sealed class SessionTerminatorIgnoreAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData[SessionTerminator.IGNORE_SESSION_TIMEOUT] = true;
            base.OnActionExecuting(filterContext);
        }
    }
}