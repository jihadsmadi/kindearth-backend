using Application;
using Core.Common;
using Core.Interfaces;
using Application.Validators;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using Application.DTOs.Auth;
using System.Security.Claims;
using Core.Enums;
using Core.Interfaces.Repositories;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Repositories;
using Infrastructure.Mapping;
using Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi(); // Swagger/OpenAPI

// Database & Identity
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure")));

// Program.cs
builder.Services.AddIdentity<AppUser, Role>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(UserProfile));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			RoleClaimType = ClaimTypes.Role,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = true,
			ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
			ValidateAudience = true,
			ValidAudience = builder.Configuration["JwtSettings:Audience"],
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
	});

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Events.OnRedirectToLogin = context =>
	{
		context.Response.StatusCode = StatusCodes.Status401Unauthorized;
		return Task.CompletedTask;
	};
});

builder.Services.AddAuthorization(options =>
{
	// Admin-only access
	options.AddPolicy("AdminPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString()));

	// Vendor + Admin access
	options.AddPolicy("VendorPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString()));

	// All authenticated users
	options.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString(), RoleType.Customer.ToString()));
});


// MediatR & Services
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterUserValidator>(); 
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger(); // Serve OpenAPI/Swagger JSON
	app.UseSwaggerUI(); // Serve Swagger UI
}

app.UseHttpsRedirection();
app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Optional: Test endpoint for root URL
app.MapGet("/", () => "E-Commerce API is running!");

app.Run();