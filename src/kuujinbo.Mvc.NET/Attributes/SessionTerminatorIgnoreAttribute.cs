using System;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.Attributes
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = true)
    ]
    public sealed class SessionTerminatorIgnoreAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.TempData[SessionTerminator.IgnoreSessionTimeout] = true;
            base.OnActionExecuting(filterContext);
        }
    }
}