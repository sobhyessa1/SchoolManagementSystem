using FluentValidation;
using project1.Application.DTOs.Notifications;

namespace project1.Application.Validation
{
    public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
    {
        public CreateNotificationRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters");

            RuleFor(x => x.RecipientRole)
                .Must(role => role == null || role == "Admin" || role == "Teacher" || role == "Student")
                .When(x => !string.IsNullOrWhiteSpace(x.RecipientRole))
                .WithMessage("RecipientRole must be Admin, Teacher, or Student");

            // Either RecipientRole, RecipientId, or ClassId must be specified
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.RecipientRole) || x.RecipientId.HasValue || x.ClassId.HasValue)
                .WithMessage("Must specify either RecipientRole, RecipientId, or ClassId");
        }
    }
}
