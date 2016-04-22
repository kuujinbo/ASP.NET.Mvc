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
        const string SEARCH_REGEX = "search[regex]";
        const string COLUMNS_DATA = "columns[{0}][data]";
        const string COLUMNS_NAME = "columns[{0}][name]";
        const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";
        const string COLUMNS_SEARCH_REGEX = "columns[{0}][search][regex]";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.BindModel(controllerContext, bindingContext);
            var request = controllerContext.HttpContext.Request;
            
            // 
            var draw = Convert.ToInt32(request[DRAW]);
            var start = Convert.ToInt32(request[START]);
            var length = Convert.ToInt32(request[LENGTH]);
            
            // 
            var search = new JqueryDataTable.Search
            {
                Value = request[SEARCH_VALUE],
            };
            
            // 
            var i = 0;
            var order = new List<JqueryDataTable.Order>();
            var orderCol = string.Format(ORDER_COLUMN, i);
            while (request[orderCol] != null)
            {
                order.Add(new JqueryDataTable.Order
                {
                    Column = Convert.ToInt32(request[orderCol]),
                    Dir = request["order[" + i + "][dir]"]
                });
                i++;
            }

            // 
            i = 0;
            var columns = new List<JqueryDataTable.Column>();
            var colName = string.Format(COLUMNS_NAME, i);
            while (request[colName] != null)
            {
                columns.Add(new JqueryDataTable.Column
                {
                    Data = request["columns[" + i + "][data]"],
                    Name = request[colName],
                    Orderable = Convert.ToBoolean(request["columns[" + i + "][orderable]"]),
                    Searchable = Convert.ToBoolean(request["columns[" + i + "][searchable]"]),
                    Search = new JqueryDataTable.Search
                    {
                        Value = request["columns[" + i + "][search][value]"]
                    }
                });
                i++;
            }

            return new JqueryDataTable
            {
                Draw = draw,
                Start = start,
                Length = length,
                HasSearchValue = columns.Where(x => !string.IsNullOrWhiteSpace(x.Search.Value)).Count() > 0,
                // Search = search,
                Orders = order,
                Columns = columns
            };
        }
    }
}