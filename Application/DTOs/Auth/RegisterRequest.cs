using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
	public record RegisterRequest(
		string Email,
		string Password,
		string FirstName,
		string LastName,
		string Phone,
		string? Role = null, 
		string? StoreName = null 
	);
}
