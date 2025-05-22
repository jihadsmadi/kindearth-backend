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
			request.LastName
		);

			// Assign "Customer" role
			if (result.IsSuccess)
				await _userRepository.AssignRoleAsync(result.Value, RoleType.Customer.ToString());

			return result;
		}
	}
}
