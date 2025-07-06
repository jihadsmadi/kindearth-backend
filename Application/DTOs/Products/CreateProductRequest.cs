using System;
using System.Collections.Generic;

namespace Application.DTOs.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Guid VendorProfileId { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ProductStockDto> Stocks { get; set; }
        public List<int> TagIds { get; set; }
    }

    public class CreateProductStockRequest
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockCount { get; set; }
    }
} 