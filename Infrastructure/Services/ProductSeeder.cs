using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class ProductSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductSeeder> _logger;

        public ProductSeeder(IUnitOfWork unitOfWork, ILogger<ProductSeeder> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var existingProducts = await _unitOfWork.Products.GetAllAsync();
                if (existingProducts.Any())
                {
                    _logger.LogInformation("Products already seeded. Skipping...");
                    return;
                }

                // Get categories and tags for seeding
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var tags = await _unitOfWork.Tags.GetAllAsync();
                var vendorProfiles = await _unitOfWork.VendorProfiles.GetAllAsync();

                if (!categories.Any() || !tags.Any() || !vendorProfiles.Any())
                {
                    _logger.LogWarning("Categories, tags, or vendor profiles not found. Skipping product seeding.");
                    return;
                }

                var maleCategory = categories.FirstOrDefault(c => c.Gender == Core.Enums.Gender.Men);
                var femaleCategory = categories.FirstOrDefault(c => c.Gender == Core.Enums.Gender.Women);
                var unisexCategory = categories.FirstOrDefault(c => c.Gender == Core.Enums.Gender.Unisex);

                var casualTag = tags.FirstOrDefault(t => t.Name == "Casual");
                var sportTag = tags.FirstOrDefault(t => t.Name == "Sport");
                var summerTag = tags.FirstOrDefault(t => t.Name == "Summer");
                var modernTag = tags.FirstOrDefault(t => t.Name == "Modern");

                var vendorProfile = vendorProfiles.First();

                var products = new List<Product>();

                // Product 1: Men's Casual T-Shirt
                if (maleCategory != null)
                {
                    var product1 = new Product
                    {
                        Name = "Men's Premium Cotton T-Shirt",
                        Description = "Comfortable and stylish cotton t-shirt perfect for everyday wear. Made from 100% organic cotton with a modern fit.",
                        CategoryId = maleCategory.Id,
                        VendorProfileId = vendorProfile.Id
                    };

                    await _unitOfWork.Products.AddAsync(product1);
                    await _unitOfWork.SaveChangesAsync();

                    // Add images
                    var product1Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = product1.Id, Url = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500" },
                        new ProductImage { ProductId = product1.Id, Url = "https://images.unsplash.com/photo-1503341504253-dff4815485f1?w=500" }
                    };

                    foreach (var image in product1Images)
                    {
                        await _unitOfWork.ProductImages.AddAsync(image);
                    }

                    // Add stocks
                    var product1Stocks = new List<ProductStock>
                    {
                        new ProductStock { ProductId = product1.Id, Size = "S", Color = "White", Quantity = 15 },
                        new ProductStock { ProductId = product1.Id, Size = "M", Color = "White", Quantity = 25 },
                        new ProductStock { ProductId = product1.Id, Size = "L", Color = "White", Quantity = 20 },
                        new ProductStock { ProductId = product1.Id, Size = "XL", Color = "White", Quantity = 10 },
                        new ProductStock { ProductId = product1.Id, Size = "M", Color = "Black", Quantity = 30 },
                        new ProductStock { ProductId = product1.Id, Size = "L", Color = "Black", Quantity = 25 }
                    };

                    foreach (var stock in product1Stocks)
                    {
                        await _unitOfWork.ProductStocks.AddAsync(stock);
                    }

                    // Add tags
                    if (casualTag != null && modernTag != null)
                    {
                        var product1Tags = new List<ProductTag>
                        {
                            new ProductTag { ProductId = product1.Id, TagId = casualTag.Id },
                            new ProductTag { ProductId = product1.Id, TagId = modernTag.Id }
                        };

                        foreach (var tag in product1Tags)
                        {
                            await _unitOfWork.ProductTags.AddAsync(tag);
                        }
                    }

                    products.Add(product1);
                }

                // Product 2: Women's Summer Dress
                if (femaleCategory != null)
                {
                    var product2 = new Product
                    {
                        Name = "Women's Floral Summer Dress",
                        Description = "Beautiful floral print dress perfect for summer occasions. Lightweight and breathable fabric with an elegant design.",
                        CategoryId = femaleCategory.Id,
                        VendorProfileId = vendorProfile.Id
                    };

                    await _unitOfWork.Products.AddAsync(product2);
                    await _unitOfWork.SaveChangesAsync();

                    // Add images
                    var product2Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = product2.Id, Url = "https://images.unsplash.com/photo-1515372039744-b8f02a3ae446?w=500" },
                        new ProductImage { ProductId = product2.Id, Url = "https://images.unsplash.com/photo-1496747611176-843222e1e57c?w=500" }
                    };

                    foreach (var image in product2Images)
                    {
                        await _unitOfWork.ProductImages.AddAsync(image);
                    }

                    // Add stocks
                    var product2Stocks = new List<ProductStock>
                    {
                        new ProductStock { ProductId = product2.Id, Size = "XS", Color = "Blue Floral", Quantity = 8 },
                        new ProductStock { ProductId = product2.Id, Size = "S", Color = "Blue Floral", Quantity = 12 },
                        new ProductStock { ProductId = product2.Id, Size = "M", Color = "Blue Floral", Quantity = 15 },
                        new ProductStock { ProductId = product2.Id, Size = "L", Color = "Blue Floral", Quantity = 10 },
                        new ProductStock { ProductId = product2.Id, Size = "S", Color = "Pink Floral", Quantity = 10 },
                        new ProductStock { ProductId = product2.Id, Size = "M", Color = "Pink Floral", Quantity = 12 }
                    };

                    foreach (var stock in product2Stocks)
                    {
                        await _unitOfWork.ProductStocks.AddAsync(stock);
                    }

                    // Add tags
                    if (summerTag != null && casualTag != null)
                    {
                        var product2Tags = new List<ProductTag>
                        {
                            new ProductTag { ProductId = product2.Id, TagId = summerTag.Id },
                            new ProductTag { ProductId = product2.Id, TagId = casualTag.Id }
                        };

                        foreach (var tag in product2Tags)
                        {
                            await _unitOfWork.ProductTags.AddAsync(tag);
                        }
                    }

                    products.Add(product2);
                }

                // Product 3: Unisex Sport Shoes
                if (unisexCategory != null)
                {
                    var product3 = new Product
                    {
                        Name = "Unisex Athletic Running Shoes",
                        Description = "High-performance running shoes designed for comfort and durability. Perfect for both casual and professional athletes.",
                        CategoryId = unisexCategory.Id,
                        VendorProfileId = vendorProfile.Id
                    };

                    await _unitOfWork.Products.AddAsync(product3);
                    await _unitOfWork.SaveChangesAsync();

                    // Add images
                    var product3Images = new List<ProductImage>
                    {
                        new ProductImage { ProductId = product3.Id, Url = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500" },
                        new ProductImage { ProductId = product3.Id, Url = "https://images.unsplash.com/photo-1608231387042-66d1773070a5?w=500" }
                    };

                    foreach (var image in product3Images)
                    {
                        await _unitOfWork.ProductImages.AddAsync(image);
                    }

                    // Add stocks
                    var product3Stocks = new List<ProductStock>
                    {
                        new ProductStock { ProductId = product3.Id, Size = "40", Color = "White", Quantity = 20 },
                        new ProductStock { ProductId = product3.Id, Size = "41", Color = "White", Quantity = 25 },
                        new ProductStock { ProductId = product3.Id, Size = "42", Color = "White", Quantity = 30 },
                        new ProductStock { ProductId = product3.Id, Size = "43", Color = "White", Quantity = 25 },
                        new ProductStock { ProductId = product3.Id, Size = "42", Color = "Black", Quantity = 20 },
                        new ProductStock { ProductId = product3.Id, Size = "43", Color = "Black", Quantity = 18 }
                    };

                    foreach (var stock in product3Stocks)
                    {
                        await _unitOfWork.ProductStocks.AddAsync(stock);
                    }

                    // Add tags
                    if (sportTag != null && modernTag != null)
                    {
                        var product3Tags = new List<ProductTag>
                        {
                            new ProductTag { ProductId = product3.Id, TagId = sportTag.Id },
                            new ProductTag { ProductId = product3.Id, TagId = modernTag.Id }
                        };

                        foreach (var tag in product3Tags)
                        {
                            await _unitOfWork.ProductTags.AddAsync(tag);
                        }
                    }

                    products.Add(product3);
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Successfully seeded {products.Count} products with images, stocks, and tags.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding products.");
                throw;
            }
        }
    }
} 