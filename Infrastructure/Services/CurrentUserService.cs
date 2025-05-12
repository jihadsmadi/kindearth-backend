using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace Infrastructure.Services
{

	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		
		public Guid? UserId
		{
			get
			{
				var userIdClaim = _httpContextAccessor.HttpContext?.User?
					.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				return Guid.TryParse(userIdClaim, out var parsedId)
					? parsedId
					: null;
			}
		}

		
		public bool IsAdmin =>
			_httpContextAccessor.HttpContext?.User?
				.IsInRole("Admin") ?? false;
	}
}
