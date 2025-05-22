using Application.DTOs.Auth;
using AutoMapper;
using Core.Common;
using Core.Entities;
using Core.Interfaces.Repositories;
using FluentValidation;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Identity.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IValidator<RegisterRequest> _registerValidator;
		private readonly IMapper _mapper;

		public UserRepository(
			UserManager<AppUser> userManager,
			RoleManager<Role> roleManager, IValidator<RegisterRequest> registerValidator, IMapper mapper)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_registerValidator = registerValidator;
			_mapper = mapper;
		}


		public async Task<Result<Guid>> RegisterUserAsync(string email, string password, string firstName, string lastName)
		{
			if (string.IsNullOrWhiteSpace(email))
				return Result<Guid>.Failure("Email is required");

			if (string.IsNullOrWhiteSpace(password))
				return Result<Guid>.Failure("Password is required");

			// Validate using FluentValidation
			var validationResult = await _registerValidator.ValidateAsync(
				new RegisterRequest(email, password, firstName, lastName));

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
				LastName = lastName
			};

			var result = await _userManager.CreateAsync(user, password);

			return result.Succeeded
				? Result<Guid>.Success(user.Id)
				: Result<Guid>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		public async Task<Result<Guid>> LoginUserAsync(string email, string password)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
				return Result<Guid>.Failure("Email and password are required");

			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
				return Result<Guid>.Failure("User not found");

			if (!await _userManager.CheckPasswordAsync(user, password))
				return Result<Guid>.Failure("Invalid credentials");

			return Result<Guid>.Success(user.Id);
		}

		public async Task<Result<string>> AssignRoleAsync(Guid userId, string role)
		{
			if (userId == Guid.Empty)
				return Result<string>.Failure("Invalid user ID");

			if (string.IsNullOrWhiteSpace(role))
				return Result<string>.Failure("Role name is required");

			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null)
				return Result<string>.Failure("User not found");

			if (!await _roleManager.RoleExistsAsync(role))
				return Result<string>.Failure("Role does not exist");


			// Remove all existing roles
			var currentRoles = await _userManager.GetRolesAsync(user);
			foreach (var r in currentRoles)
			{
				if (r == role)
					return Result<string>.Failure($"User Alredy Have The Role: {role}");
			}
			var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);


			if (!removeResult.Succeeded)
				return Result<string>.Failure("Failed to clear existing roles");

			var result = await _userManager.AddToRoleAsync(user, role);

			return result.Succeeded
				? Result<string>.Success($"Role '{role}' assigned successfully")
				: Result<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}

		public async Task<User> GetUserByIdAsync(Guid userId)
		{
			var appUser =  await _userManager.FindByIdAsync(userId.ToString());
			return _mapper.Map<User>(appUser);
		}

		public async Task<List<string>> GetUserRolesAsync(User user)
		{
			return (await _userManager.GetRolesAsync(_mapper.Map<AppUser>(user))).ToList();
		}

		public async Task<Result<bool>> UpdateUserAsync(User user)
		{
			var appUser = await _userManager.FindByIdAsync(user.Id.ToString());

			appUser.RefreshToken = user.RefreshToken;
			appUser.RefreshTokenExpiry = user.RefreshTokenExpiry;
			appUser.FirstName = user.FirstName;
			appUser.LastName = user.LastName;
			
			var result = await _userManager.UpdateAsync(appUser);
			return result.Succeeded
				? Result<bool>.Success(true)
				: Result<bool>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
		}
	}
}
