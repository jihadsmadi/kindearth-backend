using System.Security.Claims;
using Core.Common;

namespace Core.Interfaces
{
	public interface ITokenService
	{
		string GenerateJwtToken(UserDto user, IList<string> roles);
		(string RawToken, string HashedToken) GenerateRefreshTokenPair();

		ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
	}
}
