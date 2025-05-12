using Core.Entities;
using System.Security.Claims;

namespace Core.Interfaces
{
	public interface ITokenService
	{
		string GenerateJwt(User user, IList<string> roles);
		string GenerateRefreshToken();

		ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
	}
}
