namespace Core.Entities
{
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public Guid VendorProfileId { get; set; }
    public VendorProfile Vendor { get; set; }
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<ProductStock> Stocks { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; }
}
} 