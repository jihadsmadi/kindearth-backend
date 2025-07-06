using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record UpdateProductBasicInfoCommand(UpdateProductBasicInfoRequest Request) : IRequest<ProductDto>;
} 