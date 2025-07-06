using Application;
using Application.Commands.Categories;
using Application.Commands.Tags;
using Application.DTOs.Auth;
using Application.Validators;
using Core.Common;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using FluentValidation;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add core services
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database & Identity
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure")));

builder.Services.AddIdentity<AppUser, Role>()
	.AddEntityFrameworkStores<AppDbContext>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings["Issuer"],
		ValidAudience = jwtSettings["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key),
		RoleClaimType = ClaimTypes.Role
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			context.Token = context.Request.Cookies["access_token"];
			return Task.CompletedTask;
		}
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString()));

	options.AddPolicy("VendorPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString()));

	options.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString(), RoleType.Customer.ToString()));
});

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("FrontendPolicy", policy =>
	{
				policy.WithOrigins(
			   "http://localhost:3000",  // HTTP frontend
			   "https://localhost:3000")   // HTTPS frontend 
			 .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials()
			  .SetIsOriginAllowed(_ => true);
	});
});

// Application Services
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
// Store jwtSetting Configuration
builder.Services.Configure<JwtSettings>(jwtSettings);

// Validators
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterUserValidator>();
builder.Services.AddScoped<IValidator<CreateCategoryCommand>, CreateCategoryCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryCommand>, UpdateCategoryCommandValidator>();
builder.Services.AddScoped<IValidator<CreateTagCommand>, CreateTagCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateTagCommand>, UpdateTagCommandValidator>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, Infrastructure.Repositories.CategoryRepository>();
builder.Services.AddScoped<ITagRepository, Infrastructure.Repositories.TagRepository>();
builder.Services.AddScoped<IVendorProfileRepository, Infrastructure.Repositories.VendorProfileRepository>();
builder.Services.AddScoped<IProductImageRepository, Infrastructure.Repositories.ProductImageRepository>();
builder.Services.AddScoped<IProductTagRepository, Infrastructure.Repositories.ProductTagRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Infrastructure.Repositories.GenericRepository<>));
builder.Services.AddScoped<IProductStockRepository, Infrastructure.Repositories.ProductStockRepository>();
builder.Services.AddScoped<ICategorySizeRepository, Infrastructure.Repositories.CategorySizeRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, Infrastructure.Persistence.UnitOfWork>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<Application.Services.IProductMappingService, Application.Services.ProductMappingService>();
builder.Services.AddHttpContextAccessor();

// Seeders
builder.Services.AddScoped<CategorySeeder>();
builder.Services.AddScoped<TagSeeder>();
builder.Services.AddScoped<ProductSeeder>();
builder.Services.AddScoped<DataSeeder>();

builder.Services.AddAntiforgery(options =>
{
	options.HeaderName = "X-CSRF-TOKEN";
	options.Cookie.Name = "csrf-cookie";
	options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
		? CookieSecurePolicy.None
		: CookieSecurePolicy.Always; ;
	options.SuppressXFrameOptionsHeader = false;
	options.Cookie.SameSite = builder.Environment.IsDevelopment()
			? SameSiteMode.Lax  // For development
			: SameSiteMode.Strict;
	options.Cookie.HttpOnly = false;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
	try
	{
		var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
		await dataSeeder.SeedAllAsync();
	}
	catch (Exception ex)
	{
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection(); 
}
app.UseRouting();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();


// Add CSRF endpoint
app.MapGet("/csrf-token", (IAntiforgery antiforgery, HttpContext context) =>
{
	var tokens = antiforgery.GetAndStoreTokens(context);
	return Results.Ok(new { token = tokens.RequestToken });
}).AllowAnonymous();

app.MapControllers();


app.Run();