using Application.DTOs.Products;
using MediatR;

namespace Application.Queries.Products
{
    // Convert to record for better immutability
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
} 