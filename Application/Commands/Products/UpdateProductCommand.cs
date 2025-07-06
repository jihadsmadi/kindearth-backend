using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record UpdateProductCommand(UpdateProductRequest Request) : IRequest<ProductDto>;
} 