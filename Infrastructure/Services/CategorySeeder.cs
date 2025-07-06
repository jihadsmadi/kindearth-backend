using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class CategorySeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategorySeeder> _logger;

        public CategorySeeder(IUnitOfWork unitOfWork, ILogger<CategorySeeder> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var existingCategories = await _unitOfWork.Categories.GetAllAsync();
                if (existingCategories.Any())
                {
                    _logger.LogInformation("Categories already seeded. Skipping...");
                    return;
                }

                var categories = new List<Category>
                {
                    // Men's Categories
                    new Category { Name = "Shirts", ImageUrl = "https://example.com/images/shirts.jpg", Gender = Gender.Men },
                    new Category { Name = "Pants", ImageUrl = "https://example.com/images/pants.jpg", Gender = Gender.Men },
                    new Category { Name = "Shoes", ImageUrl = "https://example.com/images/shoes.jpg", Gender = Gender.Men },
                    new Category { Name = "Jackets", ImageUrl = "https://example.com/images/jackets.jpg", Gender = Gender.Men },
                    new Category { Name = "Accessories", ImageUrl = "https://example.com/images/accessories.jpg", Gender = Gender.Men },

                    // Women's Categories
                    new Category { Name = "Dresses", ImageUrl = "https://example.com/images/dresses.jpg", Gender = Gender.Women },
                    new Category { Name = "Tops", ImageUrl = "https://example.com/images/tops.jpg", Gender = Gender.Women },
                    new Category { Name = "Pants", ImageUrl = "https://example.com/images/pants.jpg", Gender = Gender.Women },
                    new Category { Name = "Shoes", ImageUrl = "https://example.com/images/shoes.jpg", Gender = Gender.Women },
                    new Category { Name = "Bags", ImageUrl = "https://example.com/images/bags.jpg", Gender = Gender.Women },
                    new Category { Name = "Accessories", ImageUrl = "https://example.com/images/accessories.jpg", Gender = Gender.Women },

                    // Boys' Categories
                    new Category { Name = "Shirts", ImageUrl = "https://example.com/images/shirts.jpg", Gender = Gender.Boys },
                    new Category { Name = "Pants", ImageUrl = "https://example.com/images/pants.jpg", Gender = Gender.Boys },
                    new Category { Name = "Shoes", ImageUrl = "https://example.com/images/shoes.jpg", Gender = Gender.Boys },

                    // Girls' Categories
                    new Category { Name = "Dresses", ImageUrl = "https://example.com/images/dresses.jpg", Gender = Gender.Girls },
                    new Category { Name = "Tops", ImageUrl = "https://example.com/images/tops.jpg", Gender = Gender.Girls },
                    new Category { Name = "Shoes", ImageUrl = "https://example.com/images/shoes.jpg", Gender = Gender.Girls },

                    // Unisex Categories
                    new Category { Name = "Hats & Caps", ImageUrl = "https://example.com/images/hats-caps.jpg", Gender = Gender.Unisex },
                    new Category { Name = "Socks", ImageUrl = "https://example.com/images/socks.jpg", Gender = Gender.Unisex },
                    new Category { Name = "Underwear", ImageUrl = "https://example.com/images/underwear.jpg", Gender = Gender.Unisex },
                    new Category { Name = "Sportswear", ImageUrl = "https://example.com/images/sportswear.jpg", Gender = Gender.Unisex },
                    new Category { Name = "Swimwear", ImageUrl = "https://example.com/images/swimwear.jpg", Gender = Gender.Unisex }
                };

                foreach (var category in categories)
                {
                    await _unitOfWork.Categories.AddAsync(category);
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Successfully seeded {categories.Count} categories.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding categories.");
                throw;
            }
        }
    }
} 