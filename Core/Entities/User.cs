﻿
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
	public class User : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiry { get; set; }
	}
}
