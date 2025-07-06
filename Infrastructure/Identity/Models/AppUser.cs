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

		

	}
}
