using Core.Enums;

namespace Application.DTOs.Categories
{
    public record CreateCategoryRequest(string Name, string? ImageUrl, Gender Gender = Gender.Unisex);
} 