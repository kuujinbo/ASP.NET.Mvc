using kuujinbo.Mvc.NET.Helpers;
using kuujinbo.Mvc.NET.Properties;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.Mvc.NET.HtmlHelpers
{
    public static class FileUploadFieldHelper
    {
        public const string ACCEPT_ALL = "*.*";
        public const string ACCEPT_FORMAT = @"
<div style='line-height:1em;font-size:0.9em'><strong>Allowed file types: [ {0} ]</strong></div>";

        /// <summary>
        /// Flag when extension called multiple times per view to ensure that
        /// JavaScript block only added once.
        /// </summary>
        public static readonly string SCRIPT_KEY = typeof(FileUploadFieldHelper).ToString();

        public static readonly string JavaScriptBlock = Resources.FileUploadField_min;

        public const string DEFAULT_BUTTON_TEXT = "Browse....";
        public const string HTML_FORMAT = @"
<div class='input-group input-group-sm'>
    <span class='input-group-btn'>
        <label class='btn btn-success' type='button'>
            <input id='fileUploadField' name='fileUploadField' style='display:none;'
                   type='file'
                   data-max-size='{0}'
                   accept='{1}'

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
            , string buttonText = DEFAULT_BUTTON_TEXT
            , string[] accept = null
            )
        {
            ScriptManagerHelper.AddInlineScript(helper, JavaScriptBlock, SCRIPT_KEY);
            ScriptManagerHelper.AddInlineScript(helper, "new FileUploadField().addListeners();");

            var maxuploadSize = WebConfigurationManagerHelper.GetMaxUploadSize(
                new WebConfigHelper(helper.ViewContext.HttpContext.Request.ApplicationPath)
            );

            return new MvcHtmlString(string.Format(
                HTML_FORMAT
                , maxuploadSize
                , accept != null ? string.Join(",", accept) : ACCEPT_ALL
                , buttonText
                , maxuploadSize / 1024
                , accept != null ? string.Format(ACCEPT_FORMAT, string.Join(",", accept)) : string.Empty
            ));
        }
    }
}