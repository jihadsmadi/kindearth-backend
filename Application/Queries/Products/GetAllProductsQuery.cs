using System.Collections.Generic;
using Application.DTOs.Products;
using MediatR;

namespace Application.Queries.Products
{
    public record GetAllProductsQuery : IRequest<List<ProductDto>>;
    
    public record GetAllProductsListQuery : IRequest<List<ProductListDto>>;
} 