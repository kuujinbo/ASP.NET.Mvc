using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Helpers;
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
        public static MvcHtmlString FileUploadField(this HtmlHelper helper, string buttonText = "Browse")
        {
            var maxuploadSize = WebConfigurationManagerHelper.GetMaxUploadSize(
                helper.ViewContext.HttpContext.Request.ApplicationPath
            );
            var html = new StringBuilder("<script type='text/javascript'>", 4096);
            html.AppendLine(Resources.FileUploadField);
            html.AppendFormat(@"
</script>
<div class='input-group'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='simpleFileUpload' name='simpleFileUpload' type='file' style='display:none;'
                   data-max-size='{0}'
                   onchange='fileUpload(this)' />{1}....
        </label>
    </span>
    <input style='pointer-events:none;background-color:#eee' type='text' required class='form-control'>
    <span class='input-group-btn' style='display:none;'>
        <button class='btn btn-danger' type='button' onclick='clearUpload(this)'><span class='glyphicon glyphicon-remove'></span></button>
    </span>
</div>
<div><strong>Max upload file size: <span style='text-decoration:underline'>{2}MB</span></strong></div>
", 
            maxuploadSize
            , buttonText
            , maxuploadSize / 1024
        );
            return new MvcHtmlString(html.ToString());
        }
    }
}