using Core.Common;
using Core.Entities;
using Core.Interfaces;
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

		
		public TokenService(IOptions<JwtSettings> jwtSettings)
		{
			_jwtSettings = jwtSettings.Value;
			Console.WriteLine(jwtSettings.Value);
		}

		public string GenerateJwt(User user, IList<string> roles)
		{
			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

		public string GenerateRefreshToken()
			=> Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
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
				ValidateLifetime = true 
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
