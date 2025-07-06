using Core.Enums;

namespace Application.DTOs.Categories
{
    public record UpdateCategoryRequest(string Name, string? ImageUrl, Gender Gender = Gender.Unisex);
} 