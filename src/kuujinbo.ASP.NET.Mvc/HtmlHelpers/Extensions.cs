using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc
{
    public static class Extensions
    {
        /// <summary>
        /// Simple file upload HTML. You **MUST** pass a `HttpPostedFileBase`
        /// with parameter name 'simpleFileUpload' to the controller action.
        /// E.g.: 
        /// public ActionResult Create(Model model, HttpPostedFileBase simpleFileUpload)
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="buttonText"></param>
        public static MvcHtmlString SimpleFileUpload(this HtmlHelper helper, string buttonText = "Browse")
        {
            var html = new StringBuilder("<script type='text/javascript'>", 4096);
            html.AppendLine(Resources.SimpleFileUpload);
            html.AppendFormat(@"
</script>
<div class='input-group'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='simpleFileUpload' name='simpleFileUpload' type='file' onchange='fileUpload(this)' style='display:none;'>{0}....
        </label>
    </span>
    <input disabled type='text' class='form-control'>
    <span class='input-group-btn' style='display:none;'>
        <button class='btn btn-danger' type='button' onclick='clearUpload(this)'><span class='glyphicon glyphicon-remove'></span></button>
    </span>
</div>", buttonText);
            return new MvcHtmlString(html.ToString());
        }
    }
}