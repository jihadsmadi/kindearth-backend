using Core.Entities;

namespace Application.DTOs.Auth
{
	public record AuthResponse(Guid id, string email, string firstName, string lastName, IEnumerable<string> roles);
}
