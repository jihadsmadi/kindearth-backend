using Core.Common;
using Core.Interfaces;
using Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Auth
{
	public class AssignRoleHandler : IRequestHandler<AssignRoleCommand, Result<string>>
	{
		private readonly IIdentityService _identityService;
		private readonly ICurrentUserService _currentUser;

		public AssignRoleHandler(
			IIdentityService identityService,
			ICurrentUserService currentUser)
		{
			_identityService = identityService;
			_currentUser = currentUser;
		}

		public async Task<Result<string>> Handle(
			AssignRoleCommand request,
			CancellationToken cancellationToken)
		{
			// Only admins can assign roles
			if (!_currentUser.IsAdmin)
				return Result<string>.Failure("Unauthorized: Admins only");


			return await _identityService.AssignRoleAsync(request.UserId, request.Role);
		}
	}
}
