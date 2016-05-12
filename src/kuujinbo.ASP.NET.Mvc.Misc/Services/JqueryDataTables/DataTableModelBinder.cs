using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables
{
    public class DataTableModelBinder : DefaultModelBinder
    {
        /* ===================================================================
         * custom data added in .js file - NOT part of jQuery DataTables API.
         * ===================================================================
         */
        public const string CHECK_COLUMN = "checkColumn";

        /* ===================================================================
         * everything from here part of jQuery DataTables API.
         * ===================================================================
         */
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
            base.BindModel(controllerContext, bindingContext);
            var request = controllerContext.HttpContext.Request.Form;

            // get base table request properties
            var draw = Convert.ToInt32(request[DRAW]);
            var start = Convert.ToInt32(request[START]);
            var length = Convert.ToInt32(request[LENGTH]);
            var checkColumn = Convert.ToBoolean(request[CHECK_COLUMN]);

            /* ===============================================================
             * jQuery DataTables regex **NOT** implemented - there's a reason 
             * the .NET Regex constructor has an overload with a timeout....
             * ===============================================================
             */
            var search = new Search
            {
                Value = request[SEARCH_VALUE],
            };

            // shift-click multiple column [de|a]scending sort request:
            var order = new List<SortOrder>();
            for (int i = 0; ; ++i)
            {
                var colOrder = request[string.Format(ORDER_COLUMN, i)];
                if (colOrder == null) break;

                var colIndex = checkColumn
                    ? Convert.ToInt32(colOrder) - 1
                    : Convert.ToInt32(colOrder);

                order.Add(new SortOrder
                {
                    Column = colIndex,
                    Direction = request[string.Format(ORDER_DIR, i)]
                });
            }

            /* ----------------------------------------------------------------
             * search and sort requests:
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
                CheckboxColumn = checkColumn,
                Search = search,
                SortOrders = order,
                Columns = columns
            };
        }
    }
}