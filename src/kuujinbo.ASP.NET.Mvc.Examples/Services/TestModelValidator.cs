using FluentValidation;
using kuujinbo.ASP.NET.Mvc.Models;

namespace kuujinbo.ASP.NET.Mvc.Examples.Services
{
    public class TestModelValidator : AbstractValidator<TestModel> 
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Position).NotNull();
            RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("Name required");
            RuleFor(x => x.Status).NotEmpty(); // .WithMessage("Please specify a status");
            RuleFor(x => x.Hobby).SetValidator(new TestHobbyValidator());
        }
    }
}