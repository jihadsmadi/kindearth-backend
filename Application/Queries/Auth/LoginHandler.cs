using Core.Common;
using Application.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Interfaces.Repositories;

namespace Application.Queries.Auth
{
	public class LoginHandler : IRequestHandler<LoginQuery, Result<AuthResponse>>
	{
		private readonly IUserRepository _userRepository;
		private readonly ITokenService _tokenService;
		private readonly ICookieService _cookieService;
		private readonly JwtSettings _jwtSettings;

		public LoginHandler(
			IUserRepository userRepository,
			ITokenService tokenService,
			ICookieService cookieService,
			IOptions<JwtSettings> jwtSettings)
		{
			_userRepository = userRepository;
			_tokenService = tokenService;
			_cookieService = cookieService;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<Result<AuthResponse>> Handle(
		LoginQuery request,
		CancellationToken cancellationToken)
		{
			// Check credentials
			var userIdResult = await _userRepository.LoginUserAsync(request.Email, request.Password);
			if (!userIdResult.IsSuccess)
				return Result<AuthResponse>.Failure(userIdResult.Error);

			// Get user details
			var user = await _userRepository.GetUserByIdAsync(userIdResult.Value);
			var roles = await _userRepository.GetUserRolesAsync(user);

			var accessToken = _tokenService.GenerateJwtToken(user, roles);
			var (refreshToken, hashedRefreshToken) = _tokenService.GenerateRefreshTokenPair();

			user.RefreshToken = hashedRefreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
			await _userRepository.UpdateUserAsync(user);

			_cookieService.SetJwtCookies(accessToken, refreshToken);

			return Result<AuthResponse>.Success(new AuthResponse(
				user.Id,
				user.Email,
				user.FirstName,
				user.LastName,
				roles));
		}
	}	
}