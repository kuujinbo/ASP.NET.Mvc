using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.Properties;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class FileUploadFieldHelper
    {
        public const string AcceptFormat = @"
<div style='line-height:1em;font-size:0.9em'><strong>Allowed file types: [ {0} ]</strong></div>";

        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string ScriptKey = typeof(FileUploadFieldHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.FileUploadField;

        public const string DefaultButtonText = "Browse....";
        public const string HtmlFormat = @"
<div class='input-group input-group-sm'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='fileUploadField' name='fileUploadField' style='display:none;'
                   type='file'
                   data-max-size='{0}' {1}

            />{2}
        </label>
    </span>
    <input tabindex='-1' style='width:90%;pointer-events:none;background-color:#eee' type='text' class='form-control'>
    <span class='input-group-btn' style='display:none;'>
        <button class='fileUploadFieldButton btn btn-danger' type='button'><span class='glyphicon glyphicon-remove'></span></button>
    </span>
</div>
<div style='line-height:1.76em;font-size:0.9em'><strong>Max file upload size [ {3:0.00} MB ]</strong></div>
{4}";

        /// <summary>
        /// File upload HTML, ONLY USE ONCE PER View. You **MUST** pass a 
        /// `HttpPostedFileBase` with parameter name 'fileUploadField' to the 
        /// controller action. E.g.: 
        /// public ActionResult Create(Model model, HttpPostedFileBase fileUploadField)
        /// </summary>
        public static MvcHtmlString FileUploadField(
            this HtmlHelper helper 
            , string buttonText = DefaultButtonText
            , string[] accept = null
            )
        {
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, ScriptKey);
            ScriptManagerHelper.AddInlineScript(helper, "new FileUploadField().addListeners();");

            var maxuploadSize = WebConfigurationManagerHelper.GetMaxUploadSize(
                new WebConfigHelper(helper.ViewContext.HttpContext.Request.ApplicationPath)
            );

            return new MvcHtmlString(string.Format(
                HtmlFormat
                , maxuploadSize
                , accept != null ? string.Format("accept='{0}'", string.Join(",", accept)) : string.Empty
                , buttonText
                , maxuploadSize / 1024
                , accept != null ? string.Format(AcceptFormat, string.Join(",", accept)) : string.Empty
            ));
        }
    }
}