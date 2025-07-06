namespace Core.Entities
{
public class ProductStock
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public int Quantity { get; set; }
}
} 