using Core.Common;
using Application.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.Auth
{
	public class LoginHandler : IRequestHandler<LoginQuery, Result<AuthResponse>>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUserRepository _UserRepository;
		private readonly ITokenService _tokenService;
		private readonly JwtSettings _jwtSettings;
		public LoginHandler(
			IHttpContextAccessor httpContextAccessor,
			IUserRepository UserRepository,
			ITokenService tokenService,
			IOptions<JwtSettings> jwtSettings)
		{
			_httpContextAccessor = httpContextAccessor;
			_UserRepository = UserRepository;
			_tokenService = tokenService;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<Result<AuthResponse>> Handle(
			LoginQuery request,
			CancellationToken cancellationToken)
		{
			// Check credentials
			var userIdResult = await _UserRepository.LoginUserAsync(request.Email, request.Password);
			if (!userIdResult.IsSuccess)
				return Result<AuthResponse>.Failure(userIdResult.Error);

			// Get user details
			var user = await _UserRepository.GetUserByIdAsync(userIdResult.Value);
			var roles = await _UserRepository.GetUserRolesAsync(user);

			// Generate tokens
			var token = _tokenService.GenerateJwt(user, roles);
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Save refresh token to the user
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
			await _UserRepository.UpdateUserAsync(user);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
			};

			_httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, cookieOptions);
			_httpContextAccessor.HttpContext?.Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);

			return Result<AuthResponse>.Success(new AuthResponse("Login Succussfully"));
		}
	}
	
}
