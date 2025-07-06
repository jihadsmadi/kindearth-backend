using Application.DTOs.Categories;
using Core.Common;
using MediatR;

namespace Application.Queries.Categories
{
    public record GetCategoryByIdQuery(int Id) : IRequest<Result<CategoryDto>>;
} 