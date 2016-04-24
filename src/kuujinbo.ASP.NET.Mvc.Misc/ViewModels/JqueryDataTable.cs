using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Attributes;
using kuujinbo.ASP.NET.Mvc.Misc.ModelBinders;
using Newtonsoft.Json;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels
{
    [ModelBinder(typeof(JqueryDataTableBinder))]
    public class JqueryDataTable
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string DataUrl { get; set; }
        public string DeleteRowUrl { get; set; }
        public string EditRowUrl { get; set; }
        /// <summary>
        /// allow client-side shift-click multiple column sorting
        /// </summary>
        public bool AllowMultiColumnSorting { get; set; }

        // TODO: implement
        // public Search Search { get; set; }

        public IEnumerable<SortOrder> SortOrders { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public IList<ActionButton> ActionButtons { get; set; }

        public JqueryDataTable()
        {
            ActionButtons = new List<ActionButton>();
            // AllowMultiColumnSorting = true;
        }

        public string GetActionButtons()
        {
            if (ActionButtons.Count > 0) 
            {
                return string.Join("", ActionButtons.Select(x => x.GetMarkUp()));
            }
            return string.Empty;
        }

        public string GetThead()
        {
            if (Columns == null || Columns.Count() < 1)
            {
                throw new ArgumentNullException("Columns");
            }

            StringBuilder s = new StringBuilder(@"
                <th style='white-space: nowrap;text-align: center !important;padding:4px !important'>
                    <input id='datatable-check-all' type='checkbox' />
                </th>"
            );
            foreach (var c in Columns)
            {
                if (c.Display) s.AppendFormat("<th>{0}</th>\n", c.Name);
            }
            s.AppendLine("<th></th>");

            return s.ToString();
        }

        public string GetTfoot()
        {
            if (Columns == null || Columns.Count() < 1)
            {
                throw new ArgumentNullException("Columns");
            }

            StringBuilder s = new StringBuilder(); 
            foreach (var c in Columns) s.AppendLine("<th></th>");
            s.AppendLine("<th style='white-space: nowrap;'></th>");

            return s.ToString();
        }

        IEnumerable<Tuple<PropertyInfo, JqueryDataTableColumnAttribute>> GetTypeInfo(Type type)
        {
            return type.GetProperties().Select(
                p => new
                {
                    prop = p,
                    col = p.GetCustomAttributes(
                        typeof(JqueryDataTableColumnAttribute), true)
                        .SingleOrDefault() as JqueryDataTableColumnAttribute
                })
                .Where(p => p.col != null)
                .OrderBy(p => p.col.DisplayOrder)
                .Select(p => new Tuple<PropertyInfo, JqueryDataTableColumnAttribute>(p.prop, p.col));
        }

        public void SetColumns<TEntity>() where TEntity : class
        {
            var columns = new List<JqueryDataTable.Column>();
            IEnumerable<Tuple<PropertyInfo, JqueryDataTableColumnAttribute>> typeInfo = GetTypeInfo(typeof(TEntity));

            foreach (var info in typeInfo)
            {
                var column = new JqueryDataTable.Column
                {
                    Name = info.Item2.DisplayName ?? info.Item1.Name,
                    Display = info.Item2.Display,
                    IsSearchable = info.Item2.IsSearchable,
                    IsSortable = info.Item2.IsSortable,
                };
                if (info.Item1.PropertyType == typeof(bool))
                {
                    column.IsSearchable = false;
                }
                columns.Add(column);
            }

            this.Columns = columns;
        }

        public object GetData<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IIdentifiable
        {
            // dictionary of <property name, <objectId, property value>>
            var propertyValueCache = new Dictionary<string, IDictionary<int, string>>();

            IEnumerable<Tuple<PropertyInfo, JqueryDataTableColumnAttribute>> typeInfo = GetTypeInfo(typeof(TEntity));

            foreach (var info in typeInfo)
            {
                propertyValueCache.Add(info.Item1.Name, new Dictionary<int, string>());
            }

            // per column search
            for (int i = 0; i < Columns.Count(); ++i)
            {
                var column = Columns.ElementAt(i);
                if (column.IsSearchable && !string.IsNullOrWhiteSpace(column.Search.Value))
                {
                    var tuple = typeInfo.ElementAt(i);
                    entities = entities.Where(e =>
                    {
                        var value = GetPropertyValue(
                            e, tuple.Item1, tuple.Item2, propertyValueCache
                        );
                        return value != null 
                            && value.ToString()
                            .IndexOf(column.Search.Value, StringComparison.OrdinalIgnoreCase) != -1;
                    });
                }
            }

            var sortedData = entities.OrderBy(r => "");
            foreach (var sortOrder in SortOrders)
            {
                var column = Columns.ElementAt(sortOrder.Column);
                if (column.IsSortable)
                {
                    var tuple = typeInfo.ElementAt(sortOrder.Column);
                    if (sortOrder.Direction == JqueryDataTableBinder.ORDER_ASC)
                    {
                        sortedData = sortedData.ThenBy(e =>
                        {
                            var val = GetPropertyValue(
                                e, tuple.Item1, tuple.Item2, propertyValueCache
                            );
                    
                            return val;
                        });
                    }
                    else
                    {
                        sortedData = sortedData.ThenByDescending(e =>
                        {
                            return GetPropertyValue(
                                e, tuple.Item1, tuple.Item2, propertyValueCache
                            );
                        });
                    }                
                }
            }

            var pagedData = sortedData.Skip(Start).Take(Length);

            // 
            var tableData = new List<List<string>>();
            foreach (var entity in pagedData)
            {
                var row = new List<string>();
                foreach (var info in typeInfo)
                {
                    row.Add(GetPropertyValue(
                        entity, info.Item1, info.Item2, propertyValueCache
                    ));
                }
                tableData.Add(row);
            }

            return new
            {
                draw = this.Draw,
                recordsTotal = entities.Count(),
                recordsFiltered = entities.Count(),
                data = tableData
            };        
        }



        protected string GetPropertyValue<TEntity>(
            TEntity entity,
            PropertyInfo propertyInfo,
            JqueryDataTableColumnAttribute fieldInfo,
            IDictionary<string, IDictionary<int, string>> cache
        ) where TEntity : class, IIdentifiable
        {
            string data = null;
            if (cache[propertyInfo.Name].TryGetValue(entity.Id, out data)) return data;

            data = string.Empty;
            var propertyIsCollection =
                propertyInfo.PropertyType != typeof(string) &&
                propertyInfo.PropertyType
                    .GetInterfaces().Any(x =>
                        x.IsGenericType &&
                        x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (propertyIsCollection)
            {
                var fields = fieldInfo.FieldAccessor.Split('.');
                var value = propertyInfo.GetValue(entity) as IEnumerable<object>;
                var items = new List<string>();
                foreach (var item in value)
                {
                    var target = item;
                    foreach (var field in fields)
                    {
                        target = item.GetType().GetProperty(field).GetValue(item);
                    }
                    items.Add(target.ToString());
                }
                data = string.Join(", ", items.OrderBy(val => val));
            }
            //else if (propertyInfo.PropertyType.IsEnum)
            //{
            //    data = RegexUtil.PascalCaseStem((propertyInfo.GetValue(entity) ?? "").ToString());
            //}
            else if (fieldInfo.FieldAccessor != null)
            {
                var value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    var fields = fieldInfo.FieldAccessor.Split('.');
                    foreach (var field in fields)
                    {
                        value = value.GetType().GetProperty(field).GetValue(value);

                        if (value == null) break;
                    }
                }
                data = (value ?? "").ToString();
            }
            else
            {
                var value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    var type = propertyInfo.PropertyType;
                    data = type == typeof(DateTime) || type == typeof(DateTime?)
                        ? ((DateTime)value).ToString("yyyy-MM-dd") : value.ToString();
                }
            }

            cache[propertyInfo.Name][entity.Id] = data;
            return data;
        }


        public string GetJavaScriptConfig()
        {
            return JsonNet.Serialize(new
                {
                    dataUrl = DataUrl,
                    deleteRowUrl = DeleteRowUrl,
                    editRowUrl = EditRowUrl,
                    allowMultiColumnSorting = AllowMultiColumnSorting,
                }
            );
        }

        /* -----------------------------------------------------------------
         * nested class - DT regex not implemented - so many ways it could go wrong
         */
        public sealed class Search
        {
            public string Value { get; set; }
        }

        /* -----------------------------------------------------------------
         * nested class
         */
        public sealed class SortOrder
        {
            public int Column { get; set; }
            public string Direction { get; set; }
        }

        /* -----------------------------------------------------------------
         * nested class
         */
        public sealed class Column
        {
            public string Data { get; set; }
            public string Name { get; set; }
            public bool Display { get; set; }
            public bool IsSortable { get; set; }
            public bool IsSearchable { get; set; }
            public Search Search { get; set; }
        }

        /* -----------------------------------------------------------------
         * nested class
         */
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
    }
}