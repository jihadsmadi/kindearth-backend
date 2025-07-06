using Core.Common;
using Core.Enums;
using MediatR;

namespace Application.Commands.Categories
{
    public record UpdateCategoryCommand(int Id, string Name, string? ImageUrl = null, Gender Gender = Gender.Unisex) : IRequest<Result<bool>>;
} 