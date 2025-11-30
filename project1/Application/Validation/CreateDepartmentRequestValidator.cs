using FluentValidation;
using project1.Application.DTOs.Admin;

namespace project1.Application.Validation
{
    public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Department name is required");
            RuleFor(x => x.Name).MaximumLength(200);
        }
    }
}
