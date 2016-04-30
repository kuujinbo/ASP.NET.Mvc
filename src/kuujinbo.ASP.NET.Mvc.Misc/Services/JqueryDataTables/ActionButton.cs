using System;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public sealed class ActionButton
    {
        /* ----------------------------------------------------------------------------
         * bootstrap classes
         * ----------------------------------------------------------------------------
         */
        public const string Primary = "btn btn-primary";
        public const string Secondary = "btn btn-secondary";
        public const string Success = "btn btn-success";
        public const string Info = "btn btn-info";
        public const string Warning = "btn btn-warning";
        public const string Danger = "btn btn-danger";
        public const string Link = "btn btn-link";

        /// <summary>
        /// default => true - otherwise hyperlink bootstrap styled as button
        /// </summary>
        public bool IsButton { get; set; }

        /// <summary>
        /// button class: default => Success
        /// </summary>
        public string CssClass { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        public ActionButton(string url, string text)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException("text");

            Url = url;
            Text = text;
            IsButton = true;
            CssClass = Success;
        }

        /// <summary>
        /// generate button markup
        /// </summary>
        /// <returns>HTML markup</returns>
        public string GetHtml()
        {
            return IsButton
                ? string.Format(
                    "<button class='{0}' data-url='{1}'>{2} <span></span></button>\n",
                    CssClass, Url, Text
                )
                : string.Format(
                    "<a class='{0}' href='{1}'>{2}</a>\n",
                    CssClass, Url, Text
                );
        }
    }
}