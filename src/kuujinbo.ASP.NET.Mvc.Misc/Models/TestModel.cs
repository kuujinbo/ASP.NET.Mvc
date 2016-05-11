using System;
using kuujinbo.ASP.NET.Mvc.Misc.Services.JqueryDataTables;

namespace kuujinbo.ASP.NET.Mvc.Misc.Models
{
    public class TestModel : IIdentifiable
    {
        public int Id { get; set; }
        [DataTableColumn(DisplayOrder = 1)]
        public string Name { get; set; }
        [DataTableColumn(DisplayOrder = 2)]
        public string Position { get; set; }
        [DataTableColumn(DisplayOrder = 3)]
        public string Office { get; set; }
        [DataTableColumn(DisplayOrder = 4)]
        public int Extension { get; set; }
        [DataTableColumn(DisplayOrder = 5, DisplayName = "Start Date")]
        public DateTime? StartDate { get; set; }
        [DataTableColumn(DisplayOrder = 6)]
        public string Salary { get; set; }
    }
}