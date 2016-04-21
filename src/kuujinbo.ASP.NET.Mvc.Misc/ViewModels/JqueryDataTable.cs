using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels
{
    public class JqueryDataTableActionButton
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

        public JqueryDataTableActionButton()
        {
            IsButton = true;
            ElementClass = Success;
            Url = "#";
        }

        public string GetMarkUp()
        {
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


    public class JqueryDataTable
    {
        public string DataUrl { get; set;}
        public string DeleteRowUrl { get; set;}
        public string EditRowUrl { get; set;}
        public int LastColumnIndex { get; set;}
        public IList<JqueryDataTableActionButton> ActionButtons { get; set; }

        public JqueryDataTable()
        {
            ActionButtons = new List<JqueryDataTableActionButton>();
        }

        public string GetActionButtons()
        {
            if (ActionButtons.Count > 0) 
            {
                return string.Join("", ActionButtons.Select(x => x.GetMarkUp()));
            }
            return string.Empty;
        }

        // TODO: write generic model / helper
        public string GetTHead()
        {
            return @"
            <th style='white-space: nowrap;text-align: center !important;padding:4px !important'>
                <input id='datatable-check-all' type='checkbox' />
            </th>
            <th>Name</th>
            <th>Position</th>
            <th>Office</th>
            <th>Extension</th>
            <th>Start date</th>
            <th>Salary</th>
            <th></th>
";
        }

        // TODO: write generic model / helper
        public string GetTFoot()
        {
            return @"
            <th></th>
            <th></th>
            <th></th>
            <th></th>
            <th></th>
            <th></th>
            <th></th>
            <th style='white-space: nowrap;'></th>
";
        }

        
        // TODO: app-level code to consistently write JSON. e.g. handle dates
        public string GetJavaScriptConfig()
        {
            return JsonConvert.SerializeObject(
                new
                {
                    dataUrl = DataUrl,
                    deleteRowUrl = DeleteRowUrl,
                    editRowUrl = EditRowUrl,
                    lastColumnIndex = LastColumnIndex
                },
                Formatting.None
            );
        }
    }
}