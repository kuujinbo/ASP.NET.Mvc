using FluentValidation.Attributes;
using System;

namespace kuujinbo.Mvc.NET.Examples.Models
{
    public enum Status
    {
        FullTime, PartTime
    }

    [Validator(typeof(TestHobbyValidator))]
    public class TestHobby
    {
        public string Name { get; set; }
    }

    [Validator(typeof(TestModelValidator))]
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

        public Status? Status { get; set; }
        public TestHobby Hobby { get; set; }
    }
}