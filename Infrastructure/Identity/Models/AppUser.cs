using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Identity.Models
{

	public class AppUser : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiry { get; set; }

		//public IEnumerable<Claim> GetClaims(IList<string> roles)
		//{
		//	var claims = new List<Claim>
		//	{
		//		new(ClaimTypes.NameIdentifier, Id.ToString()),
		//		new(ClaimTypes.Email, Email),
		//		new(ClaimTypes.GivenName, $"{FirstName} {LastName}"),
		//		new("username", UserName) 
  //          };

			
		//	foreach (var role in roles)
		//	{
		//		claims.Add(new Claim(ClaimTypes.Role, role));
		//	}

		//	return claims;
		//}

	}
}
