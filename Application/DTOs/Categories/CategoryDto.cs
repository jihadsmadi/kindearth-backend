using Core.Enums;

namespace Application.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public Gender Gender { get; set; }
    }
} 