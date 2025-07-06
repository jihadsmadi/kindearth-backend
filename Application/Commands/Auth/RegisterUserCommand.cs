using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Commands.Auth
{
	public sealed record RegisterUserCommand(
		string Email,
		string Password,
		string FirstName,
		string LastName,
		string Phone,
		string? Role = null, 
		string? StoreName = null 
	) : IRequest<Result<Guid>>;
}
