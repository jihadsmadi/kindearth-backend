using System;
using System.Collections.Generic;

namespace Application.DTOs.Products
{
    // Full update request (existing)
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ProductStockDto> Stocks { get; set; }
        public List<int> TagIds { get; set; }
    }

    // Partial update requests for specific fields
    public class UpdateProductBasicInfoRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateProductImagesRequest
    {
        public int Id { get; set; }
        public List<string> ImageUrls { get; set; }
    }

    public class UpdateProductStocksRequest
    {
        public int Id { get; set; }
        public List<ProductStockDto> Stocks { get; set; }
    }

    public class UpdateProductTagsRequest
    {
        public int Id { get; set; }
        public List<int> TagIds { get; set; }
    }

    public class UpdateProductStockRequest
    {
        public int? Id { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockCount { get; set; }
    }
} 