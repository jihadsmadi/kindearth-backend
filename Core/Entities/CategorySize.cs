namespace Core.Entities
{
public class CategorySize
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string Size { get; set; }
}
} 