using Core.Entities;
using Core.Enums;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public static class UserSeeder
	{
		public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
			var logger = serviceProvider.GetRequiredService<ILogger<object>>();

			// Seed Roles
			await SeedRoles(roleManager, logger);

			// Seed Admin User
			await SeedAdminUser(userManager, logger);
		}

		private static async Task SeedRoles(RoleManager<Role> roleManager, ILogger logger)
		{
			var roles = Enum.GetValues<RoleType>();

			foreach (var role in roles)
			{
				var roleName = role.ToString();
				if (!await roleManager.RoleExistsAsync(roleName))
				{
					var result = await roleManager.CreateAsync(new Role(roleName));
					if (result.Succeeded)
					{
						logger.LogInformation($"Role '{roleName}' created successfully");
					}
					else
					{
						logger.LogError($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
					}
				}
				else
				{
					logger.LogInformation($"Role '{roleName}' already exists");
				}
			}
		}

		private static async Task SeedAdminUser(UserManager<AppUser> userManager, ILogger logger)
		{
			const string adminEmail = "admin@kindearth.com";
			const string adminPassword = "Admin123!";

			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				adminUser = new AppUser
				{
					Email = adminEmail,
					UserName = adminEmail,
					FirstName = "System",
					LastName = "Administrator",
					EmailConfirmed = true
				};

				var createResult = await userManager.CreateAsync(adminUser, adminPassword);
				if (createResult.Succeeded)
				{
					var roleResult = await userManager.AddToRoleAsync(adminUser, RoleType.Admin.ToString());
					if (roleResult.Succeeded)
					{
						logger.LogInformation($"Admin user '{adminEmail}' created successfully with Admin role");
					}
					else
					{
						logger.LogError($"Failed to assign Admin role to user '{adminEmail}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
					}
				}
				else
				{
					logger.LogError($"Failed to create admin user '{adminEmail}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
				}
			}
			else
			{
				logger.LogInformation($"Admin user '{adminEmail}' already exists");
			}
		}

		//public static async Task SeedAdminAndVendor(IServiceProvider serviceProvider)
		//{
		//	const string adminEmail = "admin@example.com";
		//	const string adminPassword = "Admin123!";
		//	const string vendorEmail = "vendor@example.com";
		//	const string vendorPassword = "Vendor123!";

		//	var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
		//	var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

		//	// Create Admin Role if not exists
		//	if (!await roleManager.RoleExistsAsync("Admin"))
		//		await roleManager.CreateAsync(new Role("Admin"));

		//	// Create Vendor Role if not exists
		//	if (!await roleManager.RoleExistsAsync("Vendor"))
		//		await roleManager.CreateAsync(new Role("Vendor"));

		//	// Create Admin User
		//	var adminUser = await userManager.FindByEmailAsync(adminEmail);
		//	if (adminUser == null)
		//	{
		//		adminUser = new User
		//		{
		//			Email = adminEmail,
		//			UserName = adminEmail,
		//			FirstName = "Admin",
		//			LastName = "User"
		//		};

		//		var createResult = await userManager.CreateAsync(adminUser, adminPassword);
		//		if (createResult.Succeeded)
		//			await userManager.AddToRoleAsync(adminUser, "Admin");
		//	}

		//	// Create Vendor User
		//	var vendorUser = await userManager.FindByEmailAsync(vendorEmail);
		//	if (vendorUser == null)
		//	{
		//		vendorUser = new User
		//		{
		//			Email = vendorEmail,
		//			UserName = vendorEmail,
		//			FirstName = "Vendor",
		//			LastName = "User"
		//		};

		//		var createResult = await userManager.CreateAsync(vendorUser, vendorPassword);
		//		if (createResult.Succeeded)
		//			await userManager.AddToRoleAsync(vendorUser, "Vendor");
		//	}
		//}

	}
}