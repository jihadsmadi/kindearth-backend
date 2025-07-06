using System;
using System.Collections.Generic;

namespace Application.DTOs.Products
{
    // Lightweight DTO for product listings (better performance)
    public class ProductListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid VendorProfileId { get; set; }
        public string VendorStoreName { get; set; }
        public string MainImageUrl { get; set; } 
        public int TotalStock { get; set; } 
        public List<string> Tags { get; set; }
    }

    // Full DTO for detailed product view
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid VendorProfileId { get; set; }
        public string VendorStoreName { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ProductStockDto> Stocks { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ProductStockDto
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
    }
} 