

using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Commands.Auth
{
	public sealed record RegisterUserCommand(
		string Email,
		string Password,
		string FirstName,
		string LastName
	) : IRequest<Result<Guid>>;
}
