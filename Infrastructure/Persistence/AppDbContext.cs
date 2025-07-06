using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<AppUser, Role, Guid>
	{
		public DbSet<VendorProfile> VendorProfiles { get; set; }
		
		// Product Catalog Entities
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<ProductStock> ProductStocks { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<ProductTag> ProductTags { get; set; }
		public DbSet<CategorySize> CategorySizes { get; set; }

		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configure AppUser entity
			builder.Entity<AppUser>(entity =>
			{
				entity.Property(u => u.FirstName).HasMaxLength(50);
				entity.Property(u => u.LastName).HasMaxLength(50);
				entity.Property(u => u.RefreshToken).HasMaxLength(512);
			});

			// Configure VendorProfile entity
			builder.Entity<VendorProfile>(entity =>
			{
				entity.HasKey(vp => vp.Id);
				entity.Property(vp => vp.StoreName).HasMaxLength(100).IsRequired();
			});

			// Configure Product entity
			builder.Entity<Product>(entity =>
			{
				entity.HasKey(p => p.Id);
				entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
				entity.Property(p => p.Description).HasMaxLength(1000);
				
				entity.HasOne(p => p.Category)
					.WithMany()
					.HasForeignKey(p => p.CategoryId)
					.OnDelete(DeleteBehavior.Restrict);
				
				entity.HasOne(p => p.Vendor)
					.WithMany()
					.HasForeignKey(p => p.VendorProfileId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Category entity
			builder.Entity<Category>(entity =>
			{
				entity.HasKey(c => c.Id);
				entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
				entity.Property(c => c.ImageUrl).HasMaxLength(500);
				entity.Property(c => c.Gender).HasConversion<int>().IsRequired();
			});

			// Configure Tag entity
			builder.Entity<Tag>(entity =>
			{
				entity.HasKey(t => t.Id);
				entity.Property(t => t.Name).HasMaxLength(50).IsRequired();
			});

			// Configure ProductStock entity
			builder.Entity<ProductStock>(entity =>
			{
				entity.HasKey(ps => ps.Id);
				entity.Property(ps => ps.Size).HasMaxLength(20).IsRequired();
				entity.Property(ps => ps.Color).HasMaxLength(50).IsRequired();
				entity.Property(ps => ps.Quantity).IsRequired();
				
				entity.HasOne(ps => ps.Product)
					.WithMany(p => p.Stocks)
					.HasForeignKey(ps => ps.ProductId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configure ProductImage entity
			builder.Entity<ProductImage>(entity =>
			{
				entity.HasKey(pi => pi.Id);
				entity.Property(pi => pi.Url).HasMaxLength(500).IsRequired();
				
				entity.HasOne(pi => pi.Product)
					.WithMany(p => p.Images)
					.HasForeignKey(pi => pi.ProductId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configure ProductTag entity (many-to-many)
			builder.Entity<ProductTag>(entity =>
			{
				entity.HasKey(pt => new { pt.ProductId, pt.TagId });
				
				entity.HasOne(pt => pt.Product)
					.WithMany(p => p.ProductTags)
					.HasForeignKey(pt => pt.ProductId)
					.OnDelete(DeleteBehavior.Cascade);
				
				entity.HasOne(pt => pt.Tag)
					.WithMany()
					.HasForeignKey(pt => pt.TagId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configure CategorySize entity
			builder.Entity<CategorySize>(entity =>
			{
				entity.HasKey(cs => cs.Id);
				entity.Property(cs => cs.Size).HasMaxLength(20).IsRequired();
				
				entity.HasOne(cs => cs.Category)
					.WithMany()
					.HasForeignKey(cs => cs.CategoryId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
