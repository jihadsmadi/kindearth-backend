using Application.Commands.Tags;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
    {
        public UpdateTagCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Tag ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tag name is required")
                .MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters")
                .Matches("^[a-zA-Z0-9\\s\\-]+$").WithMessage("Tag name can only contain letters, numbers, spaces, and hyphens");
        }
    }
} 