using Core.Common;
using Application.DTOs.Auth;
using Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

using Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

			// Create claims identity
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
					new Claim(ClaimTypes.Role, string.Join(",", roles))
				};

			// Create authentication properties
			var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
			{
				IsPersistent = true,
				ExpiresUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
				AllowRefresh = true
			};

			// Create claims principal
			var claimsIdentity = new ClaimsIdentity(
				claims,
				CookieAuthenticationDefaults.AuthenticationScheme);

			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

			// Sign in using built-in authentication
			await _httpContextAccessor.HttpContext!.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				claimsPrincipal,
				authProperties);

			return Result<AuthResponse>.Success(new AuthResponse("Login Successful"));
		}
	}

}
