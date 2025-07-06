using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record UpdateProductImagesCommand(UpdateProductImagesRequest Request) : IRequest<ProductDto>;
} 