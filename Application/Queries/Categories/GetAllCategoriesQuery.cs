using Application.DTOs.Categories;
using Core.Common;
using MediatR;

namespace Application.Queries.Categories
{
    public record GetAllCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>;
} 