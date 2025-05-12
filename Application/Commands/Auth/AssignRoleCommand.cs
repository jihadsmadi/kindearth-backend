using Core.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Auth
{
	public record AssignRoleCommand(Guid UserId,string Role) : IRequest<Result<string>>;

}
