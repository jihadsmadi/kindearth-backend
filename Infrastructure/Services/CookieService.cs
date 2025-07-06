using Core.Common;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class CookieService : ICookieService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly JwtSettings _jwtSettings;
		private readonly IHostEnvironment _environment;

		public CookieService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings, IHostEnvironment environment)
		{
			this._httpContextAccessor = httpContextAccessor;
			this._jwtSettings = jwtSettings.Value;
			this._environment = environment;
		}
		public void SetJwtCookies(string accessToken, string refreshToken)
		{
			var response = _httpContextAccessor.HttpContext?.Response;
			if (response == null) return;

			var accessOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = !_environment.IsDevelopment(), // Only secure in production
				SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict, // Lax for development, Strict for production
				Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
			};

			var refreshOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = !_environment.IsDevelopment(), // Only secure in production
				SameSite = _environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict, // Lax for development, Strict for production
				Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
			};

			response.Cookies.Append("access_token", accessToken, accessOptions);
			response.Cookies.Append("refresh_token", refreshToken, refreshOptions);
		}

		public bool RemoveJwtCookies()
		{
			var response = _httpContextAccessor.HttpContext?.Response;
			if (response == null) return false;

			response?.Cookies.Delete("access_token");
			response?.Cookies.Delete("refresh_token");
			return true;
		}
	}
}
