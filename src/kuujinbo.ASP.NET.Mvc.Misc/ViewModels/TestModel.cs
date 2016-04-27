using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using kuujinbo.ASP.NET.Mvc.Misc.Services;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels
{
    public interface IIdentifiable { int Id { get; } }

    public class TestModel : IIdentifiable
    {
        [DataTableColumn(
            Display = false, DisplayOrder = 0, 
            IsSearchable=false, IsSortable=false)
        ]
        public int Id { get; set; }
        [DataTableColumn(DisplayOrder = 1)]
        public string Name { get; set; }
        [DataTableColumn(DisplayOrder = 2)]
        public string Position { get; set; }
        [DataTableColumn(DisplayOrder = 3)]
        public string Office { get; set; }
        [DataTableColumn(DisplayOrder = 4)]
        public int Extension { get; set; }
        [DataTableColumn(DisplayOrder = 5, DisplayName="Start Date")]
        public DateTime? StartDate { get; set; }
        [DataTableColumn(DisplayOrder = 6)]
        public string Salary { get; set; }
    }
}