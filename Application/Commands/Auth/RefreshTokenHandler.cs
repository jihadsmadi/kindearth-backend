using Core.Common;
using Core.DTOs.Auth;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Auth
{
	public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
	{
		private readonly ITokenService _tokenService;
		private readonly UserManager<User> _userManager;
		private readonly JwtSettings _jwtSettings;

		public RefreshTokenHandler(
			ITokenService tokenService,
			UserManager<User> userManager,
			IOptions<JwtSettings> jwtSettings)
		{
			_tokenService = tokenService;
			_userManager = userManager;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<Result<AuthResponse>> Handle(
			RefreshTokenCommand request,
			CancellationToken cancellationToken)
		{
			// Get principal from expired token
			var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
			if (principal == null)
				return Result<AuthResponse>.Failure("Invalid token");

			// Get user
			var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return Result<AuthResponse>.Failure("User not found");

			// Validate refresh token
			if (user.RefreshToken != request.RefreshToken)
				return Result<AuthResponse>.Failure("Invalid refresh token");

			if (user.RefreshTokenExpiry < DateTime.UtcNow)
				return Result<AuthResponse>.Failure("Refresh token expired");

			// Generate new tokens
			var roles = await _userManager.GetRolesAsync(user);
			var newToken = _tokenService.GenerateJwt(user, roles);
			var newRefreshToken = _tokenService.GenerateRefreshToken();

			// Update user
			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);

			return Result<AuthResponse>.Success(new AuthResponse(newToken, newRefreshToken));
		}
	}

}
