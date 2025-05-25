using Application;
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
using Infrastructure.Mapping;
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
		policy.WithOrigins("http://localhost:3000")
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

// Mapper Configuration
builder.Services.AddAutoMapper(typeof(UserProfile));
// Application Services
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));
// Store jwtSetting Configuration
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterUserValidator>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery(options =>
{
	options.HeaderName = "X-CSRF-TOKEN";
	options.Cookie.Name = "csrf-cookie";
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
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