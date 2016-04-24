using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using kuujinbo.ASP.NET.Mvc.Misc.Attributes;

namespace kuujinbo.ASP.NET.Mvc.Misc.ViewModels
{
    public interface IIdentifiable { int Id { get; } }

    public class TestModel : IIdentifiable
    {
        [JqueryDataTableColumn(Display = false, DisplayOrder = 0)]
        public int Id { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 1)]
        public string Name { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 2)]
        public string Position { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 3)]
        public string Office { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 4)]
        public int Extension { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 5)]
        public DateTime StartDate { get; set; }
        [JqueryDataTableColumn(DisplayOrder = 6)]
        public string Salary { get; set; }
    }
}