
using Core.Entities;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<AppUser, Role, Guid> 
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }


		
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configure entity relationships here
			builder.Entity<User>(entity =>
			{
				entity.HasIndex(u => u.Email).IsUnique();
				entity.Property(u => u.FirstName).HasMaxLength(50);
				entity.Property(u => u.LastName).HasMaxLength(50);
				entity.Property(u => u.RefreshToken).HasMaxLength(512);
				entity.Property(u => u.RefreshTokenExpiry);
			});
		}
	}

}
