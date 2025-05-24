using Application.Validators;
using Application;
using Core.Common;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Repositories;
using Infrastructure.Mapping;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Database & Identity
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure")));

// Identity Configuration
builder.Services.AddIdentity<AppUser, Role>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

// Repository and AutoMapper
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(UserProfile));

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);

// Cookie Authentication Configuration
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
		? CookieSecurePolicy.None
		: CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Strict;
	options.ExpireTimeSpan = TimeSpan.FromDays(7);// refactor
});

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("kindearth-frontend", policy =>
	{
		policy.WithOrigins(builder.Configuration["AllowedOrigins"]!.Split(';'))
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

// Authorization Policies (Keep existing)
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString()));

	options.AddPolicy("VendorPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString()));

	options.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole(RoleType.Admin.ToString(), RoleType.Vendor.ToString(), RoleType.Customer.ToString()));
});

// CSRF Protection
builder.Services.AddAntiforgery(options =>
{
	options.HeaderName = "X-CSRF-TOKEN";
	options.SuppressXFrameOptionsHeader = false;
});

// Other Services (Keep existing)
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
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Security Middleware
app.UseCors("kindearth-frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "E-Commerce API is running!");

app.Run();