using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Commands.Tags
{
    public record UpdateTagCommand(int Id, string Name) : IRequest<Result<bool>>;
} 