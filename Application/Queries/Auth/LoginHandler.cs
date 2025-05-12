using Core.Common;
using Core.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Auth
{
	public class LoginHandler : IRequestHandler<LoginQuery, Result<AuthResponse>>
	{
		private readonly IIdentityService _identityService;
		private readonly ITokenService _tokenService;
		private readonly JwtSettings _jwtSettings;
		public LoginHandler(
			IIdentityService identityService,
			ITokenService tokenService,
			IOptions<JwtSettings> jwtSettings)
		{
			_identityService = identityService;
			_tokenService = tokenService;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<Result<AuthResponse>> Handle(
			LoginQuery request,
			CancellationToken cancellationToken)
		{
			// Check credentials
			var userIdResult = await _identityService.LoginUserAsync(request.Email, request.Password);
			if (!userIdResult.IsSuccess)
				return Result<AuthResponse>.Failure(userIdResult.Error);

			// Get user details
			var user = await _identityService.GetUserByIdAsync(userIdResult.Value);
			var roles = await _identityService.GetUserRolesAsync(user);

			// Generate tokens
			var token = _tokenService.GenerateJwt(user, roles);
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Save refresh token to the user
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
			await _identityService.UpdateUserAsync(user);

			return Result<AuthResponse>.Success(new AuthResponse(token, refreshToken));
		}
	}
	
}
