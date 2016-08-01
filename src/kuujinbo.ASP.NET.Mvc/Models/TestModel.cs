using kuujinbo.ASP.NET.Mvc.Services.JqueryDataTables;
using System;

namespace kuujinbo.ASP.NET.Mvc.Models
{
    public enum Status
    {
        FullTime, PartTime
    }

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
        [DataTableColumn(DisplayOrder = 7)]
        public bool? Salaried { get; set; }
        [DataTableColumn(DisplayOrder = 7)]
        public Status Status { get; set; }
    }
}