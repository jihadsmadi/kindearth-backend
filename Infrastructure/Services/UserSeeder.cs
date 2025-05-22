using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public static class UserSeeder
	{
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