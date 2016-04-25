using System;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels.JqueryDataTables
{
    public sealed class ActionButton
    {
        public const string Primary = "btn btn-primary";
        public const string Secondary = "btn btn-secondary";
        public const string Success = "btn btn-success";
        public const string Info = "btn btn-info";
        public const string Warning = "btn btn-warning";
        public const string Danger = "btn btn-danger";
        public const string Link = "btn btn-link";

        public bool IsButton { get; set; }
        public string ElementClass { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }

        public ActionButton()
        {
            IsButton = true;
            ElementClass = Success;
        }

        public string GetMarkUp()
        {
            if (string.IsNullOrWhiteSpace(Url)) throw new ArgumentNullException("Url");

            return IsButton ?
                string.Format(
                    "<button class='{0}' data-url='{1}'>{2} <span></span></button>\n",
                    ElementClass, Url, Text
                )
                : string.Format(
                    "<a class='{0}' href='{1}'>{2}</a>\n",
                    ElementClass, Url, Text
                );
        }
    }
}