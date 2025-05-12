using Application.Commands.Auth;
using Application.Queries.Auth;
using Core.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AuthController(IMediator mediator) => _mediator = mediator;

		[HttpPost("register")]
		[AllowAnonymous]
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
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var query = new LoginQuery(request.Email, request.Password);
			var result = await _mediator.Send(query);
			return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
		}

		[HttpPost("refreshToken")]
		[AllowAnonymous]
		public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
		{
			var command = new RefreshTokenCommand(request.Token, request.RefreshToken);
			var result = await _mediator.Send(command);
			return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
		}

		[Authorize(Policy = "AdminPolicy")]
		[HttpPost("assignRole")]
		public async Task<IActionResult> AssignRole(AssignRoleRequest request)
		{
			var command = new AssignRoleCommand(request.UserId, request.Role);
			var result = await _mediator.Send(command);
			return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
		}

		[HttpGet("test")]
		[Authorize(Policy = "AdminPolicy")]
		public async Task<IActionResult> Test()
		{
			return Ok("test admin");
		}
	}
}
