using Core.Common;
using Core.DTOs.Auth;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Auth
{
	public record RefreshTokenCommand(
			string Token,
			string RefreshToken
		) : IRequest<Result<AuthResponse>>;
}
