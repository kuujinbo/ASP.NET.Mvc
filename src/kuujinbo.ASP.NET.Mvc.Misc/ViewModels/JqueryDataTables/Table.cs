using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using kuujinbo.ASP.NET.Mvc.Misc.Attributes;
using kuujinbo.ASP.NET.Mvc.Misc.ModelBinders;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels.JqueryDataTables
{
    [ModelBinder(typeof(JqueryDataTablesBinder))]
    public class Table
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

        public Table()
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
            foreach (var c in Columns)
            {
                s.AppendFormat(
                    "<th data-is-searchable='{0}'></th>",
                    c.IsSearchable ? c.IsSearchable.ToString().ToLower() : string.Empty
                );
            }
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
            var columns = new List<Column>();
            IEnumerable<Tuple<PropertyInfo, JqueryDataTableColumnAttribute>> typeInfo = GetTypeInfo(typeof(TEntity));

            foreach (var info in typeInfo)
            {
                var column = new Column
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
            var propertyValueCache = new Dictionary<string, IDictionary<int, object>>();

            IEnumerable<Tuple<PropertyInfo, JqueryDataTableColumnAttribute>> typeInfo = GetTypeInfo(typeof(TEntity));

            foreach (var info in typeInfo)
            {
                propertyValueCache.Add(info.Item1.Name, new Dictionary<int, object>());
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
                    if (sortOrder.Direction == JqueryDataTablesBinder.ORDER_ASC)
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
            var tableData = new List<List<object>>();
            foreach (var entity in pagedData)
            {
                var row = new List<object>();
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



        protected object GetPropertyValue<TEntity>(
            TEntity entity,
            PropertyInfo propertyInfo,
            JqueryDataTableColumnAttribute fieldInfo,
            IDictionary<string, IDictionary<int, object>> cache
        ) where TEntity : class, IIdentifiable
        {
            object data = null;
            if (cache[propertyInfo.Name].TryGetValue(entity.Id, out data)) return data;

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
                data = value;
            }
            else
            {
                var value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    var type = propertyInfo.PropertyType;
                    data = value;
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


    }
}