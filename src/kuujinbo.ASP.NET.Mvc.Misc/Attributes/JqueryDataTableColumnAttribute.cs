using System;

namespace kuujinbo.ASP.NET.Mvc.Misc.Attributes
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field, 
        AllowMultiple = false, 
        Inherited = true)
    ]
    public sealed class JqueryDataTableColumnAttribute : Attribute
    {
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }

        public JqueryDataTableColumnAttribute()
        {
            IsSearchable = true;
            IsSortable = true;
        }
    }
}