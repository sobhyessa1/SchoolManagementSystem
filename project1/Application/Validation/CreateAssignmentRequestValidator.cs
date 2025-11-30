using FluentValidation;
using project1.Application.DTOs.Teacher;

namespace project1.Application.Validation
{
    public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequest>
    {
        public CreateAssignmentRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");
        }
    }
}
