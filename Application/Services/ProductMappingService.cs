using System.Collections.Generic;
using System.Linq;
using Application.DTOs.Products;
using Core.Entities;

namespace Application.Services
{
    public interface IProductMappingService
    {
        ProductDto MapToProductDto(Product product);
        ProductListDto MapToProductListDto(Product product);
        List<ProductDto> MapToProductDtoList(IEnumerable<Product> products);
        List<ProductListDto> MapToProductListDtoList(IEnumerable<Product> products);
    }

    public class ProductMappingService : IProductMappingService
    {
        public ProductDto MapToProductDto(Product product)
        {
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                VendorProfileId = product.VendorProfileId,
                VendorStoreName = product.Vendor?.StoreName,
                ImageUrls = product.Images?.Select(i => i.Url).ToList() ?? new List<string>(),
                Stocks = product.Stocks?.Select(s => new ProductStockDto
                {
                    Size = s.Size,
                    Color = s.Color,
                    Quantity = s.Quantity
                }).ToList() ?? new List<ProductStockDto>(),
                Tags = product.ProductTags?.Select(pt => pt.Tag.Name).ToList() ?? new List<string>()
            };
        }

        public ProductListDto MapToProductListDto(Product product)
        {
            if (product == null) return null;

            return new ProductListDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                VendorProfileId = product.VendorProfileId,
                VendorStoreName = product.Vendor?.StoreName,
                MainImageUrl = product.Images?.FirstOrDefault()?.Url,
                TotalStock = product.Stocks?.Sum(s => s.Quantity) ?? 0,
                Tags = product.ProductTags?.Select(pt => pt.Tag.Name).ToList() ?? new List<string>()
            };
        }

        public List<ProductDto> MapToProductDtoList(IEnumerable<Product> products)
        {
            return products?.Select(MapToProductDto).ToList() ?? new List<ProductDto>();
        }

        public List<ProductListDto> MapToProductListDtoList(IEnumerable<Product> products)
        {
            return products?.Select(MapToProductListDto).ToList() ?? new List<ProductListDto>();
        }
    }
} 