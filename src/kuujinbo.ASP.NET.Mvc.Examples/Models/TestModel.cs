using System;

namespace kuujinbo.ASP.NET.Mvc.Models
{
    public enum Status
    {
        FullTime, PartTime
    }
    //[Validator(typeof(TestModelValidator))]
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Office { get; set; }
        public int Extension { get; set; }
        public DateTime? StartDate { get; set; }
        public string Salary { get; set; }
        public bool? Salaried { get; set; }
        public Status Status { get; set; }
    }
}