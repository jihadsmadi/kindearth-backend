using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductDto>;
} 