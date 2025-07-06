using Application.DTOs.Auth;
using Core.Common;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using FluentValidation;
using Infrastructure.Identity.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Core.Entities;

namespace Infrastructure.Identity.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IValidator<RegisterRequest> _registerValidator;

		public UserRepository(
			UserManager<AppUser> userManager,
			RoleManager<Role> roleManager, 
			IValidator<RegisterRequest> registerValidator)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_registerValidator = registerValidator;
		}

		public async Task<Result<Guid>> RegisterUserAsync(string email, string password, string firstName, string lastName, string phone)
		{
			if (string.IsNullOrWhiteSpace(email))
				return Result<Guid>.Failure("Email is required");
			if (string.IsNullOrWhiteSpace(password))
				return Result<Guid>.Failure("Password is required");
			if (string.IsNullOrWhiteSpace(phone))
				return Result<Guid>.Failure("Phone is required");

			// Validate using FluentValidation
			var validationResult = await _registerValidator.ValidateAsync(
				new RegisterRequest(email, password, firstName, lastName, phone, null, null));
			if (!validationResult.IsValid)
			{
				return Result<Guid>.Failure(
					string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
			}

			var user = new AppUser
			{
				Email = email,
				UserName = email,
				FirstName = firstName,
				LastName = lastName,
				PhoneNumber = phone
			};
			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
				return Result<Guid>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

			return Result<Guid>.Success(user.Id);
		}

		public async Task<Result<Guid>> LoginUserAsync(string email, string password)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
				return Result<Guid>.Failure("Email and password are required");

			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
				return Result<Guid>.Failure("Invalid email or password");

			if (!await _userManager.CheckPasswordAsync(user, password))
				return Result<Guid>.Failure("Invalid email or password");

			return Result<Guid>.Success(user.Id);
		}

		public async Task<Result<string>> AssignRoleAsync(Guid userId, string role)
		{
			if (userId == Guid.Empty)
				return Result<string>.Failure("Invalid user ID");

			if (string.IsNullOrWhiteSpace(role))
				return Result<string>.Failure("Role name is required");

			if (!await _roleManager.RoleExistsAsync(role))
				return Result<string>.Failure($"Role '{role}' does not exist");

			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
				return Result<string>.Failure("User not found");

			// Check if user already has the role
			var currentRoles = await _userManager.GetRolesAsync(user);
			if (currentRoles.Contains(role))
				return Result<string>.Success($"User already has the role: {role}");

			// Add the role without removing existing ones
			var result = await _userManager.AddToRoleAsync(user, role);

			return result.Succeeded
				? Result<string>.Success($"Role '{role}' assigned successfully")
				: Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		public async Task<Result<string>> ReplaceUserRoleAsync(Guid userId, string newRole)
		{
			if (userId == Guid.Empty)
				return Result<string>.Failure("Invalid user ID");

			if (string.IsNullOrWhiteSpace(newRole))
				return Result<string>.Failure("Role name is required");

			if (!await _roleManager.RoleExistsAsync(newRole))
				return Result<string>.Failure($"Role '{newRole}' does not exist");

			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
				return Result<string>.Failure("User not found");

			// Get current roles
			var currentRoles = await _userManager.GetRolesAsync(user);
			
			// Remove all existing roles
			if (currentRoles.Any())
			{
				var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
				if (!removeResult.Succeeded)
					return Result<string>.Failure("Failed to clear existing roles");
			}

			// Add the new role
			var result = await _userManager.AddToRoleAsync(user, newRole);

			return result.Succeeded
				? Result<string>.Success($"Role replaced with '{newRole}' successfully")
				: Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		private UserDto MapToUserDto(AppUser user)
		{
			if (user == null) return null;
			return new UserDto
			{
				Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Phone = user.PhoneNumber,
				RefreshToken = user.RefreshToken,
				RefreshTokenExpiry = user.RefreshTokenExpiry
			};
		}

		public async Task<UserDto> GetUserByIdAsync(Guid userId)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			return MapToUserDto(user);
		}

		public async Task<List<string>> GetUserRolesAsync(UserDto userDto)
		{
			var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
			return user == null ? new List<string>() : (await _userManager.GetRolesAsync(user)).ToList();
		}

		public async Task<Result<bool>> UpdateUserAsync(UserDto userDto)
		{
			var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
			if (user == null) return Result<bool>.Failure("User not found");
			user.FirstName = userDto.FirstName;
			user.LastName = userDto.LastName;
			user.RefreshToken = userDto.RefreshToken;
			user.RefreshTokenExpiry = userDto.RefreshTokenExpiry;
			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded
				? Result<bool>.Success(true)
				: Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		public bool VerifyRefreshToken(UserDto userDto, string rawToken)
		{
			// This assumes RefreshToken is hashed, adjust if not
			return _userManager.PasswordHasher.VerifyHashedPassword(
				null,
				userDto.RefreshToken,
				rawToken
			) == PasswordVerificationResult.Success;
		}

		public async Task<Result<bool>> CreateVendorProfileAsync(Guid userId, string storeName)
		{
			// This method would need to be implemented differently
			// For now, return success as the vendor profile creation
			// should be handled by a separate service or command
			return Result<bool>.Success(true);
		}
	}
}
