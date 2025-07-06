using Core.Common;
using Application.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Core.Interfaces.Repositories;

namespace Application.Commands.Auth
{
	public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
	{
		private readonly ICookieService _cookieService;
		private readonly ITokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly JwtSettings _jwtSettings;

		public RefreshTokenHandler(
			ICookieService cookieService,
			ITokenService tokenService,
			IUserRepository userRepository,
			IOptions<JwtSettings> jwtSettings
			)
		{
			_cookieService = cookieService;
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
			if (!_userRepository.VerifyRefreshToken(user, request.RefreshToken))
				return Result<AuthResponse>.Failure("Invalid refresh token");

			if (user.RefreshTokenExpiry < DateTime.UtcNow)
			{
				_cookieService.RemoveJwtCookies();
				return Result<AuthResponse>.Failure("Refresh token expired");
			}

			// Generate new tokens
			var roles = await _userRepository.GetUserRolesAsync(user);
			var newToken = _tokenService.GenerateJwtToken(user, roles);
			var (rawRefreshToken, hashedRefreshToken) = _tokenService.GenerateRefreshTokenPair();

			user.RefreshToken = hashedRefreshToken;
			await _userRepository.UpdateUserAsync(user);

			_cookieService.SetJwtCookies(newToken, rawRefreshToken);

			return Result<AuthResponse>.Success(new AuthResponse(
				user.Id, user.Email, user.FirstName, user.LastName, roles));
		}
	}
}
