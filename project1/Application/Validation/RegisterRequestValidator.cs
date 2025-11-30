using FluentValidation;
using project1.Application.DTOs.Auth;
using System.Text.RegularExpressions;

namespace project1.Application.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
                .Must(HasUpper).WithMessage("Password must contain an uppercase letter")
                .Must(HasLower).WithMessage("Password must contain a lowercase letter")
                .Must(HasDigit).WithMessage("Password must contain a digit")
                .Must(HasSpecial).WithMessage("Password must contain a special character");

            // Ensure students can register themselves only (role must be Student)
            RuleFor(x => x.Role).Must(r => r == project1.Domain.Enums.Role.Student).When(x => true).WithMessage("Public registration is allowed for Students only. Admin/Teacher accounts must be created by Admin.");
        }

        private bool HasUpper(string password) => password.Any(char.IsUpper);
        private bool HasLower(string password) => password.Any(char.IsLower);
        private bool HasDigit(string password) => password.Any(char.IsDigit);
        private bool HasSpecial(string password) => Regex.IsMatch(password, "[^a-zA-Z0-9]");
    }
}
