using Core.Common;
using Application.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Core.Entities;
using Core.Interfaces.Repositories;

namespace Application.Commands.Auth
{
	
	public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
	{
		private readonly ITokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly JwtSettings _jwtSettings;

		public RefreshTokenHandler(
			ITokenService tokenService,
			IUserRepository userRepository,
			IOptions<JwtSettings> jwtSettings
			)
		{
			_tokenService = tokenService;
			_userRepository = userRepository;
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
			var identifier = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			Guid userId = new Guid();
			if(Guid.TryParse(identifier, out var id))
				userId = id;

			var user = await _userRepository.GetUserByIdAsync(userId);
			if (user == null)
				return Result<AuthResponse>.Failure("User not found");

			// Validate refresh token
			if (user.RefreshToken != request.RefreshToken)
				return Result<AuthResponse>.Failure("Invalid refresh token");

			if (user.RefreshTokenExpiry < DateTime.UtcNow)
				return Result<AuthResponse>.Failure("Refresh token expired");

			// Generate new tokens
			var roles = await _userRepository.GetUserRolesAsync(user);
			var newToken = _tokenService.GenerateJwt(user, roles);
			var newRefreshToken = _tokenService.GenerateRefreshToken();

			// Update user
			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
			await _userRepository.UpdateUserAsync(user);

			return Result<AuthResponse>.Success(new AuthResponse(""));
		}
	}

}
