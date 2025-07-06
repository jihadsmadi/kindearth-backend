using Core.Common;
using MediatR;

namespace Application.Commands.Tags
{
    public record CreateTagCommand(string Name) : IRequest<Result<int>>;
} 