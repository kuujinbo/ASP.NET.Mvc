using FluentValidation;
using kuujinbo.ASP.NET.Mvc.Models;

namespace kuujinbo.ASP.NET.Mvc.Examples.Services
{
    public class TestHobbyValidator : AbstractValidator<TestHobby> 
    {
        public TestHobbyValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty();
        }
    }
}