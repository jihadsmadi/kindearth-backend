using Core.Common;
using MediatR;

namespace Application.Commands.Categories
{
    public record DeleteCategoryCommand(int Id) : IRequest<Result<bool>>;
} 