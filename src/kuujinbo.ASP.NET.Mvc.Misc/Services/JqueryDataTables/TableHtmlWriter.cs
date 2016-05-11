/* ---------------------------------------------------------------------------
 * HTML/JavaScript written to Partial View:
 * ~/views/shared/_jQueryDataTables.cshtml
 * ---------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public partial class Table
    {
        public string ActionButtonsHtml()
        {
            return ActionButtons.Count > 0
                ? string.Join("", ActionButtons.Select(x => x.GetHtml()))
                : string.Empty;
        }

        public string GetTableHtml()
        {
            if (Columns == null || Columns.Count() < 1)
            {
                throw new ArgumentNullException("Columns");
            }

            var showCheckboxColumn = ShowCheckboxColumn();
            StringBuilder s = new StringBuilder("<thead><tr>");
            GetTheadHtml(s, showCheckboxColumn);
            s.AppendLine("</tr></thead>");

            s.AppendLine("<tfoot><tr>");
            GetTfootHtml(s, showCheckboxColumn);
            s.AppendLine("</tr></tfoot>");

            return s.ToString();
        }

        private void GetTheadHtml(StringBuilder s, bool showCheckboxColumn)
        {
            if (showCheckboxColumn)
            {
                s.AppendLine(@"
                <th style='white-space: nowrap;text-align: center !important;padding:4px !important'>
                    <input id='datatable-check-all' type='checkbox' />
                </th>"
                );
            }

            foreach (var c in Columns) s.AppendFormat("<th>{0}</th>\n", c.Name);

            s.AppendLine("<th></th>");
        }

        private void GetTfootHtml(StringBuilder s, bool showCheckboxColumn)
        {
            if (showCheckboxColumn) s.AppendLine("<th></th>");
            foreach (var c in Columns)
            {
                s.AppendFormat(
                    "<th data-is-searchable='{0}'></th>\n",
                    c.IsSearchable ? c.IsSearchable.ToString().ToLower() : string.Empty
                );
            }
            s.AppendLine("<th style='white-space: nowrap;'></th>");
        }

        public string GetJavaScriptConfig()
        {
            if (string.IsNullOrEmpty(DataUrl))
                throw new ArgumentNullException("DataUrl");

            return JsonNetSerializer.Get(new
            {
                dataUrl = DataUrl,
                deleteRowUrl = DeleteRowUrl,
                editRowUrl = EditRowUrl,
                showCheckboxColumn = ShowCheckboxColumn(),
                allowMultiColumnSorting = AllowMultiColumnSorting
            });
        }
    }
}