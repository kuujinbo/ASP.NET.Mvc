using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Helpers;
using kuujinbo.ASP.NET.Mvc.Properties;

namespace kuujinbo.ASP.NET.Mvc
{
    public static class FileUploadFieldExtension
    {
        /// <summary>
        /// File upload HTML, ONLY USE ONCE PER View. You **MUST** pass a 
        /// `HttpPostedFileBase` with parameter name 'fileUploadField' to the 
        /// controller action. E.g.: 
        /// public ActionResult Create(Model model, HttpPostedFileBase fileUploadField)
        /// </summary>
        public static MvcHtmlString FileUploadField(this HtmlHelper helper, string buttonText = "Browse")
        {
            var maxuploadSize = WebConfigurationManagerHelper.GetMaxUploadSize(
                helper.ViewContext.HttpContext.Request.ApplicationPath
            );
            var html = new StringBuilder("<script type='text/javascript'>", 4096);
            html.AppendLine(Resources.FileUploadField);
            html.AppendFormat(@"</script>
<div class='input-group input-group-sm'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='fileUploadField' name='fileUploadField' type='file' style='display:none;'
                   data-max-size='{0}' />{1}....
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
</script>"
            , maxuploadSize
            , buttonText
            , maxuploadSize / 1024
        );
            return new MvcHtmlString(html.ToString());
        }
    }
}