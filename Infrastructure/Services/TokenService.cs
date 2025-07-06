using Core.Common;
using Core.Interfaces;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
	public class TokenService : ITokenService
	{
		private readonly JwtSettings _jwtSettings;
		private readonly UserManager<AppUser> _userManager;

		public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<AppUser> userManager)
		{
			_jwtSettings = jwtSettings.Value;
			this._userManager = userManager;
		}

		public string GenerateJwtToken(UserDto user, IList<string> roles)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
				new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
			};

			// Add roles to the token
			foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public (string RawToken, string HashedToken) GenerateRefreshTokenPair()
		{
			var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			var hashedToken = _userManager.PasswordHasher.HashPassword(null, rawToken);
			return (rawToken, hashedToken);
		}

		public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidAudience = _jwtSettings.Audience,
				ValidateIssuer = true,
				ValidIssuer = _jwtSettings.Issuer,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
				ValidateLifetime = false 
			};

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

				if (securityToken is not JwtSecurityToken jwtSecurityToken ||
					!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
						StringComparison.InvariantCultureIgnoreCase))
					return null;

				return principal;
			}
			catch
			{
				return null;
			}
		}
	}
}
