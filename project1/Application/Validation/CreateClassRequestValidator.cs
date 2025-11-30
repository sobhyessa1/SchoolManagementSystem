using FluentValidation;
using project1.Application.DTOs.Teacher;

namespace project1.Application.Validation
{
    public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
    {
        public CreateClassRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");
        }
    }
}
