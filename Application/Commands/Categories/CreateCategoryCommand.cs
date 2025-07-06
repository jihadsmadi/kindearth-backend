using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Commands.Categories
{
    public record CreateCategoryCommand(string Name, string? ImageUrl = null, Gender Gender = Gender.Unisex) : IRequest<Result<int>>;
} 