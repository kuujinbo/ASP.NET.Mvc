using FluentValidation;
using System.Diagnostics.CodeAnalysis;


namespace kuujinbo.Mvc.NET.Examples.Models
{
    [ExcludeFromCodeCoverage]
    public class TestModelValidator : AbstractValidator<TestModel> 
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Position).NotNull().WithMessage("Required");
            RuleFor(x => x.Name).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("Required");
            RuleFor(x => x.Status).NotEmpty().WithMessage("Required");
            RuleFor(x => x.Hobby).SetValidator(new TestHobbyValidator());
        }
    }

    [ExcludeFromCodeCoverage]
    public class TestHobbyValidator : AbstractValidator<TestHobby>
    {
        public TestHobbyValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Required");
        }
    }
}