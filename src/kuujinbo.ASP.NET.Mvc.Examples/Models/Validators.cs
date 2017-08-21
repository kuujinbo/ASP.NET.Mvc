using FluentValidation;

namespace kuujinbo.ASP.NET.Mvc.Examples.Models
{
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

    public class TestHobbyValidator : AbstractValidator<TestHobby>
    {
        public TestHobbyValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Required");
        }
    }
}