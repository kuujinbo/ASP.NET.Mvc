using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.ViewModels;

namespace kuujinbo.ASP.NET.Mvc.Misc.ModelBinders
{
    public class JqueryDataTableBinder : DefaultModelBinder
    {
        const string DRAW = "draw";
        const string START = "start";
        const string LENGTH = "length";

        const string ORDER_COLUMN = "order[{0}][column]";
        const string ORDER_DIR = "order[{0}][dir]";
        const string ORDER_ASC = "asc";
        const string ORDER_DESC = "desc";

        const string SEARCH_VALUE = "search[value]";
        const string COLUMNS_DATA = "columns[{0}][data]";
        const string COLUMNS_NAME = "columns[{0}][name]";
        const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.BindModel(controllerContext, bindingContext);
            var request = controllerContext.HttpContext.Request;
            
            // 
            var draw = Convert.ToInt32(request[DRAW]);
            var start = Convert.ToInt32(request[START]);
            var length = Convert.ToInt32(request[LENGTH]);

            // DT regex not implemented - so many ways it could go wrong
            var search = new JqueryDataTable.Search
            {
                Value = request[SEARCH_VALUE],
            };
            
            // 
            var order = new List<JqueryDataTable.SortOrder>();
            for (int i = 0; ; ++i)
            {
                var colOrder = request[string.Format(ORDER_COLUMN, i)];
                if (colOrder == null) break;

                order.Add(new JqueryDataTable.SortOrder
                {
                    Column = Convert.ToInt32(colOrder),
                    Direction = request["order[" + i + "][dir]"]
                });
            }

            // 
            var columns = new List<JqueryDataTable.Column>();
            for (int i = 0; ; ++i)
            {
                var colName = request[string.Format(COLUMNS_NAME, i)];
                if (colName == null) break;

                var searchable = Convert.ToBoolean(request["columns[" + i + "][searchable]"]);
                var orderable = Convert.ToBoolean(request["columns[" + i + "][orderable]"]);
                columns.Add(new JqueryDataTable.Column
                {
                    Data = request["columns[" + i + "][data]"],
                    Name = request[string.Format(COLUMNS_NAME, i)],
                    IsSortable = orderable,
                    IsSearchable = Convert.ToBoolean(request["columns[" + i + "][searchable]"]),
                    Search = searchable ? new JqueryDataTable.Search
                    {
                        Value = request["columns[" + i + "][search][value]"]
                    } : null
                });
            }

            return new JqueryDataTable
            {
                Draw = draw,
                Start = start,
                Length = length,
                // HasSearchValue = columns.Where(x => !string.IsNullOrWhiteSpace(x.Search.Value)).Count() > 0,
                // Search = search,
                SortOrders = order,
                Columns = columns
            };
        }
    }
}