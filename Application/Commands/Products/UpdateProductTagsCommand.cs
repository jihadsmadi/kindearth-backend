using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record UpdateProductTagsCommand(UpdateProductTagsRequest Request) : IRequest<ProductDto>;
} 