using System;
using MediatR;
using Core.Entities;
using Core.Common;
using Core.Interfaces;
using Core.Enums;
using Core.Interfaces.Repositories;

namespace Application.Commands.Auth
{
	public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
	{
		private readonly IUserRepository _userRepository;

		public RegisterUserHandler(IUserRepository userRepository)
			=> _userRepository = userRepository;

		public async Task<Result<Guid>> Handle(
			RegisterUserCommand request,
			CancellationToken cancellationToken
		)
		{
			var result = await _userRepository.RegisterUserAsync(
				request.Email,
				request.Password,
				request.FirstName,
				request.LastName,
				request.Phone
			);

			if (!result.IsSuccess)
				return result;

			// Use the role from the request, default to Customer if not specified
			var roleToAssign = !string.IsNullOrWhiteSpace(request.Role) 
				? request.Role 
				: "Customer";

			// Assign role
			var roleResult = await _userRepository.AssignRoleAsync(result.Value, roleToAssign);
			if (!roleResult.IsSuccess)
				return Result<Guid>.Success(result.Value);

			// Create VendorProfile if user is a vendor
			if (roleToAssign == "Vendor" && !string.IsNullOrEmpty(request.StoreName))
			{
				await _userRepository.CreateVendorProfileAsync(result.Value, request.StoreName);
			}

			return result;
		}
	}
}
