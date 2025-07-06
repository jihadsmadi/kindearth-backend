using Application.Commands.Categories;
using Core.Enums;
using FluentValidation;

namespace Application.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");
    }
} 