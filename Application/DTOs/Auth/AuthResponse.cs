using Core.Entities;

namespace Application.DTOs.Auth
{
	public record AuthResponse(Guid userId, string email,string fName,string lName,IEnumerable<string> roles);
}
