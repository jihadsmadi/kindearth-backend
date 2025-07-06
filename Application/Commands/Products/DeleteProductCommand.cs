using MediatR;

namespace Application.Commands.Products
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
} 