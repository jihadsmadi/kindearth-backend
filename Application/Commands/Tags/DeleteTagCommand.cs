using Core.Common;
using MediatR;

namespace Application.Commands.Tags
{
    public record DeleteTagCommand(int Id) : IRequest<Result<bool>>;
} 