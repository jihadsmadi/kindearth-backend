using Application.DTOs.Categories;
using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Queries.Categories
{
    public record GetCategoriesByGenderQuery(Gender Gender) : IRequest<Result<IEnumerable<CategoryDto>>>;
} 