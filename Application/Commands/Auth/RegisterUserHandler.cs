using System;
using MediatR;
using Core.Entities;
using Core.Common;
using Core.Interfaces;
using Core.Enums;


namespace Application.Commands.Auth
{

	public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
	{
		private readonly IIdentityService _identityService;

		public RegisterUserHandler(IIdentityService identityService)
			=> _identityService = identityService;

		public async Task<Result<Guid>> Handle(
			RegisterUserCommand request,
			CancellationToken cancellationToken
		)
		{
			var result = await _identityService.RegisterUserAsync(
			request.Email,
			request.Password,
			request.FirstName,
			request.LastName
		);

			// Assign "Customer" role
			if (result.IsSuccess)
				await _identityService.AssignRoleAsync(result.Value, RoleType.Customer.ToString());

			return result;
		}
	}
}
