using Application.DTOs.Products;
using MediatR;

namespace Application.Commands.Products
{
    public record UpdateProductStocksCommand(UpdateProductStocksRequest Request) : IRequest<ProductDto>;
} 