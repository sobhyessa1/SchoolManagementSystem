using FluentValidation;
using project1.Application.DTOs.Admin;

namespace project1.Application.Validation
{
    public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
    {
        public CreateCourseRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Credits).GreaterThan(0);
        }
    }
}
