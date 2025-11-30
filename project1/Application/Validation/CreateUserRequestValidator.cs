using FluentValidation;
using project1.Application.DTOs.Admin;
using System.Text.RegularExpressions;

namespace project1.Application.Validation
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Valid email is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Must(HasUpper).WithMessage("Password must contain an uppercase letter")
                .Must(HasLower).WithMessage("Password must contain a lowercase letter")
                .Must(HasDigit).WithMessage("Password must contain a digit")
                .Must(HasSpecial).WithMessage("Password must contain a special character");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role");
        }

        private bool HasUpper(string password) => password.Any(char.IsUpper);
        private bool HasLower(string password) => password.Any(char.IsLower);
        private bool HasDigit(string password) => password.Any(char.IsDigit);
        private bool HasSpecial(string password) => Regex.IsMatch(password, "[^a-zA-Z0-9]");
    }
}
