using Application.Commands.Categories;
using Core.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters")
                .Matches("^[a-zA-Z0-9\\s\\-]+$").WithMessage("Category name can only contain letters, numbers, spaces, and hyphens");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value");
        }
    }
} 