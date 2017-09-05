using kuujinbo.ASP.NET.Mvc.Helpers;
using kuujinbo.ASP.NET.Mvc.Properties;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.HtmlHelpers
{
    public static class FileUploadFieldHelper
    {
        public static readonly string JavaScriptBlock;
        static FileUploadFieldHelper()
        {
            var script = new StringBuilder("<script type='text/javascript'>", 4096);
            script.AppendLine(Resources.FileUploadField);
            script.AppendLine("</script>");
            JavaScriptBlock = script.ToString();
        }
        
        public const string DEFAULT_BUTTON_TEXT = "Browse....";
        public const string HTML_FORMAT = @"
<div class='input-group input-group-sm'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='fileUploadField' name='fileUploadField' type='file' style='display:none;'
                   data-max-size='{0}' />{1}
        </label>
    </span>
    <input tabindex='-1' style='width:90%;pointer-events:none;background-color:#eee' type='text' required class='form-control'>
    <span class='input-group-btn' style='display:none;'>
        <button class='fileUploadFieldButton btn btn-danger' type='button'><span class='glyphicon glyphicon-remove'></span></button>
    </span>
</div>
<div><strong>Max upload file size: <span style='text-decoration:underline'>{2}MB</span></strong></div>
<script type='text/javascript'>
    var fu = new FileUploadField();
    fu.addListeners();
</script>";

        /// <summary>
        /// File upload HTML, ONLY USE ONCE PER View. You **MUST** pass a 
        /// `HttpPostedFileBase` with parameter name 'fileUploadField' to the 
        /// controller action. E.g.: 
        /// public ActionResult Create(Model model, HttpPostedFileBase fileUploadField)
        /// </summary>
        public static MvcHtmlString FileUploadField(
            this HtmlHelper helper, 
            string buttonText = DEFAULT_BUTTON_TEXT)
        {
            var html = new StringBuilder(JavaScriptBlock, 4096);
            var maxuploadSize = WebConfigurationManagerHelper.GetMaxUploadSize(
                new WebConfigHelper(helper.ViewContext.HttpContext.Request.ApplicationPath)
            );
            html.AppendFormat(
                HTML_FORMAT
                , maxuploadSize
                , buttonText
                , maxuploadSize / 1024
            );
            return new MvcHtmlString(html.ToString());
        }
    }
}