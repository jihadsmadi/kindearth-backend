using Core.Common;
using Core.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Auth
{
	public record LoginQuery(
		string Email,
		string Password
	) : IRequest<Result<AuthResponse>>;
}
