using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CustomersController : ControllerBase
	{
		[Authorize(Policy = "CustomerPolicy")]
		[HttpGet("test")]
		public async Task<IActionResult> test()
		{
			await Task.Delay(1000);
			return Ok("Getting Customers");
		}
	}
}
