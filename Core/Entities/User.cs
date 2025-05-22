
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiry { get; set; }
	}
}
