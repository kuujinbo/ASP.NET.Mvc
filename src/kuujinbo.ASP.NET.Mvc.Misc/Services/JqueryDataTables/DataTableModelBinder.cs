using System;
using System.Collections.Generic;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public class DataTableModelBinder : DefaultModelBinder
    {
        public const string DRAW = "draw";
        public const string START = "start";
        public const string LENGTH = "length";

        public const string SEARCH_VALUE = "search[value]";

        public const string ORDER_ASC = "asc";
        public const string ORDER_DIR = "order[{0}][dir]";
        public const string ORDER_COLUMN = "order[{0}][column]";

        public const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        public const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        public const string COLUMNS_DATA = "columns[{0}][data]";
        public const string COLUMNS_NAME = "columns[{0}][name]";
        public const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // base.BindModel(controllerContext, bindingContext);
            var request = controllerContext.HttpContext.Request.Form;
            
            // get base table request properties
            var draw = Convert.ToInt32(request[DRAW]);
            var start = Convert.ToInt32(request[START]);
            var length = Convert.ToInt32(request[LENGTH]);

            // jQuery DataTables regex not implemented - too many ways it could go wrong
            var search = new Search
            {
                Value = request[SEARCH_VALUE],
            };

            // get [de|a]scending per-column sort requests:
            var order = new List<SortOrder>();
            for (int i = 0; ; ++i)
            {
                var colOrder = request[string.Format(ORDER_COLUMN, i)];
                if (colOrder == null) break;

                order.Add(new SortOrder
                {
                    Column = Convert.ToInt32(colOrder),
                    Direction = request[string.Format(ORDER_DIR, i)]
                });
            }

            /* ----------------------------------------------------------------
             * get search and sort requests:
             * jQuery DataTables regex not implemented - too many ways it could go wrong
             * ----------------------------------------------------------------
             */
            var columns = new List<Column>();
            for (int i = 0; ; ++i)
            {
                var colName = request[string.Format(COLUMNS_NAME, i)];
                if (colName == null) break;

                var searchable = Convert.ToBoolean(request[string.Format(COLUMNS_SEARCHABLE, i)]);
                var orderable = Convert.ToBoolean(request[string.Format(COLUMNS_ORDERABLE, i)]);

                columns.Add(new Column
                {
                    Data = request[string.Format(COLUMNS_DATA, i)],
                    Name = request[string.Format(COLUMNS_NAME, i)],
                    IsSearchable = searchable,
                    IsSortable = orderable,
                    Search = searchable ? new Search
                    {
                        Value = request[string.Format(COLUMNS_SEARCH_VALUE, i)]
                    } : null
                });
            }

            return new Table
            {
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                SortOrders = order,
                Columns = columns
            };
        }
    }
}