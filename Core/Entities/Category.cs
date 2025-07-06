namespace Core.Entities
{
using Core.Enums;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public Gender Gender { get; set; } = Gender.Unisex;
}
} 