using Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
	public class RegisterUserValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterUserValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email format");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters")
				.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
				.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
				.Matches("[0-9]").WithMessage("Password must contain at least one number")
				.Matches("[!@#$%^&*()_+\\-=\\[\\]{};':\"\\|,.<>\\/?]").WithMessage("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;':\",./<>?)");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required")
				.MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required")
				.MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone is required");

			RuleFor(x => x.Role)
				.Must(role => string.IsNullOrEmpty(role) || role == "Customer" || role == "Vendor")
				.WithMessage("Role must be 'Customer' or 'Vendor'");

			RuleFor(x => x.StoreName)
				.NotEmpty().When(x => x.Role == "Vendor")
				.WithMessage("Store name is required for vendors");
		}
	}
}
