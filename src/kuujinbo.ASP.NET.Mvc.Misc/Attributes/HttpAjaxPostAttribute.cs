using System.Reflection;

namespace System.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpAjaxPostAttribute : ActionMethodSelectorAttribute
	{
		private static readonly 
            AcceptVerbsAttribute _innerAttribute = new AcceptVerbsAttribute(HttpVerbs.Post);

		/// <summary>
        /// Determines whether the action method ajax post request is valid for the specified controller context.
        /// </summary>
		/// <returns>
        /// true if the action method request is valid for the specified controller context; otherwise, false.
        /// </returns>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="methodInfo">Information about the action method.</param>
		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
            return controllerContext.HttpContext.Request.IsAjaxRequest()
                && 
                HttpAjaxPostAttribute._innerAttribute.IsValidForRequest(controllerContext, methodInfo);
		}
    }
}