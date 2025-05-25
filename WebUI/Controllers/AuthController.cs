using Application.Commands.Auth;
using Application.Queries.Auth;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Common;
using Microsoft.Extensions.Options;

namespace WebUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly JwtSettings _jwtSettings;

		public AuthController(IMediator mediator, IOptions<JwtSettings> jwtSettings)
		{
			_mediator = mediator;
			_jwtSettings = jwtSettings.Value;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var command = new RegisterUserCommand(
				request.Email,
				request.Password,
				request.FirstName,
				request.LastName);

			var result = await _mediator.Send(command);
			return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var query = new LoginQuery(request.Email, request.Password);
			var result = await _mediator.Send(query);
			return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
		}

		[HttpPost("refreshToken")]
		[AllowAnonymous]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshToken = Request.Cookies["refresh_token"];
			var accessToken = Request.Cookies["access_token"];

			var command = new RefreshTokenCommand(accessToken, refreshToken);
			var result = await _mediator.Send(command);

			if (!result.IsSuccess) return Unauthorized();

			// Set new cookies
			//Response.Cookies.Append("access_token", result.Value.newToken, new CookieOptions
			//{
			//	HttpOnly = true,
			//	Secure = true,
			//	SameSite = SameSiteMode.Strict,
			//	Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
			//});

			return Ok("token refresh done.");
		}

		[Authorize(Policy = "AdminPolicy")]
		[HttpPost("assignRole")]
		public async Task<IActionResult> AssignRole(AssignRoleRequest request)
		{
			var command = new AssignRoleCommand(request.UserId, request.Role);
			var result = await _mediator.Send(command);
			return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
		}

		[Authorize(Policy ="CustomerPolicy")]
		[HttpPost("logout")]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("access_token");
			Response.Cookies.Delete("refresh_token");
			return Ok();
		}

		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdminPolicy")]
		[HttpPost("adminTest")]
		public async Task<IActionResult> adminTest()
		{
			await Task.Delay(1000);
			return Ok("adminTest");
		}

		[Authorize(Policy = "VendorPolicy")]
		[HttpPost("vendorTest")]
		public async Task<IActionResult> vendorTest()
		{
			await Task.Delay(1000);
			return Ok("vendorTest");
		}

		[ValidateAntiForgeryToken]
		[HttpPost("customerTest")]
		public async Task<IActionResult> customerTest()
		{
			await Task.Delay(1000);
			return Ok("customerTest");
		}
	}
}
