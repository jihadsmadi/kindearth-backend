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
			try
			{
				var command = new RegisterUserCommand(
					request.Email,
					request.Password,
					request.FirstName,
					request.LastName,
					request.Phone,
					request.Role,
					request.StoreName
				);
				var result = await _mediator.Send(command);
				if (result.IsSuccess)
				{
					return Ok(ApiResponse<Guid>.SuccessResult(
						result.Value,
						"User registered successfully"));
				}
				return BadRequest(ApiResponse.FailureResult(
					"Registration failed",
					new List<string> { result.Error }));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred during registration",
					new List<string> { ex.Message }));
			}
		}

		[HttpPost("login")]
		[AllowAnonymous]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			try
			{
				var query = new LoginQuery(request.Email, request.Password);
				var result = await _mediator.Send(query);
				
				if (result.IsSuccess)
				{
					return Ok(ApiResponse<AuthResponse>.SuccessResult(
						result.Value, 
						"Login successful"));
				}
				
				return Unauthorized(ApiResponse.FailureResult(
					"Login failed", 
					new List<string> { result.Error }));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred during login",
					new List<string> { ex.Message }));
			}
		}

		[HttpPost("refreshToken")]
		[AllowAnonymous]
		public async Task<IActionResult> RefreshToken()
		{
			try
			{
				var refreshToken = Request.Cookies["refresh_token"];
				if (string.IsNullOrEmpty(refreshToken))
					return BadRequest(ApiResponse.FailureResult(
						"Refresh token is required"));

				var accessToken = Request.Cookies["access_token"];
				if (string.IsNullOrEmpty(accessToken))
					return BadRequest(ApiResponse.FailureResult(
						"Access token is required"));

				var command = new RefreshTokenCommand(accessToken, refreshToken);
				var result = await _mediator.Send(command);

				if (!result.IsSuccess) 
					return Unauthorized(ApiResponse.FailureResult(
						"Token refresh failed", 
						new List<string> { result.Error }));

				return Ok(ApiResponse.SuccessResult("Token refreshed successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred during token refresh",
					new List<string> { ex.Message }));
			}
		}

		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdminPolicy")]
		[HttpPost("assignRole")]
		public async Task<IActionResult> AssignRole(AssignRoleRequest request)
		{
			try
			{
				var command = new AssignRoleCommand(request.UserId, request.Role);
				var result = await _mediator.Send(command);
				
				if (result.IsSuccess)
				{
					return Ok(ApiResponse<string>.SuccessResult(
						result.Value, 
						"Role assigned successfully"));
				}
				
				return BadRequest(ApiResponse.FailureResult(
					"Role assignment failed", 
					new List<string> { result.Error }));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred during role assignment",
					new List<string> { ex.Message }));
			}
		}

		[ValidateAntiForgeryToken]
		[Authorize(Policy ="CustomerPolicy")]
		[HttpPost("logout")]
		public IActionResult Logout()
		{
			try
			{
				Response.Cookies.Delete("access_token");
				Response.Cookies.Delete("refresh_token");
				return Ok(ApiResponse.SuccessResult("Logged out successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred during logout",
					new List<string> { ex.Message }));
			}
		}

		[ValidateAntiForgeryToken]
		[Authorize(Policy = "AdminPolicy")]
		[HttpPost("adminTest")]
		public async Task<IActionResult> adminTest()
		{
			await Task.Delay(1000);
			return Ok(ApiResponse.SuccessResult("Admin test successful"));
		}

		[ValidateAntiForgeryToken]
		[Authorize(Policy = "VendorPolicy")]
		[HttpPost("vendorTest")]
		public async Task<IActionResult> vendorTest()
		{
			await Task.Delay(1000);
			return Ok(ApiResponse.SuccessResult("Vendor test successful"));
		}

		[ValidateAntiForgeryToken]
		[HttpPost("customerTest")]
		public async Task<IActionResult> customerTest()
		{
			await Task.Delay(1000);
			return Ok(ApiResponse.SuccessResult("Customer test successful"));
		}

		[Authorize]
		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			try
			{
				var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
				{
					return Unauthorized(ApiResponse.FailureResult("Invalid user token"));
				}

				// Get user details from repository
				var userRepository = HttpContext.RequestServices.GetRequiredService<Core.Interfaces.Repositories.IUserRepository>();
				var user = await userRepository.GetUserByIdAsync(userGuid);
				var roles = await userRepository.GetUserRolesAsync(user);

				if (user == null)
				{
					return NotFound(ApiResponse.FailureResult("User not found"));
				}

				var profile = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, roles);
				return Ok(ApiResponse<AuthResponse>.SuccessResult(profile, "Profile retrieved successfully"));
			}
			catch (Exception ex)
			{
				return StatusCode(500, ApiResponse.FailureResult(
					"An unexpected error occurred while retrieving profile",
					new List<string> { ex.Message }));
			}
		}
	}
}
