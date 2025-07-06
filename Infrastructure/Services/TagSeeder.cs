using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class TagSeeder
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TagSeeder> _logger;

        public TagSeeder(IUnitOfWork unitOfWork, ILogger<TagSeeder> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var existingTags = await _unitOfWork.Tags.GetAllAsync();
                if (existingTags.Any())
                {
                    _logger.LogInformation("Tags already seeded. Skipping...");
                    return;
                }

                var tags = new List<Tag>
                {
                    // Material Tags
                    new Tag { Name = "Cotton" },
                    new Tag { Name = "Polyester" },
                    new Tag { Name = "Wool" },
                    new Tag { Name = "Leather" },
                    new Tag { Name = "Denim" },
                    new Tag { Name = "Silk" },
                    new Tag { Name = "Linen" },
                    new Tag { Name = "Synthetic" },

                    // Style Tags
                    new Tag { Name = "Casual" },
                    new Tag { Name = "Formal" },
                    new Tag { Name = "Sporty" },
                    new Tag { Name = "Vintage" },
                    new Tag { Name = "Modern" },
                    new Tag { Name = "Classic" },
                    new Tag { Name = "Trendy" },
                    new Tag { Name = "Bohemian" },
                    new Tag { Name = "Minimalist" },

                    // Color Tags
                    new Tag { Name = "Black" },
                    new Tag { Name = "White" },
                    new Tag { Name = "Blue" },
                    new Tag { Name = "Red" },
                    new Tag { Name = "Green" },
                    new Tag { Name = "Yellow" },
                    new Tag { Name = "Pink" },
                    new Tag { Name = "Purple" },
                    new Tag { Name = "Brown" },
                    new Tag { Name = "Gray" },
                    new Tag { Name = "Orange" },

                    // Size Tags
                    new Tag { Name = "XS" },
                    new Tag { Name = "S" },
                    new Tag { Name = "M" },
                    new Tag { Name = "L" },
                    new Tag { Name = "XL" },
                    new Tag { Name = "XXL" },
                    new Tag { Name = "Plus Size" },

                    // Feature Tags
                    new Tag { Name = "Eco-Friendly" },
                    new Tag { Name = "Sustainable" },
                    new Tag { Name = "Organic" },
                    new Tag { Name = "Waterproof" },
                    new Tag { Name = "Breathable" },
                    new Tag { Name = "Quick-Dry" },
                    new Tag { Name = "Wrinkle-Free" },
                    new Tag { Name = "Stain-Resistant" },
                    new Tag { Name = "UV Protection" },
                    new Tag { Name = "Anti-Bacterial" },

                    // Occasion Tags
                    new Tag { Name = "Work" },
                    new Tag { Name = "Party" },
                    new Tag { Name = "Wedding" },
                    new Tag { Name = "Beach" },
                    new Tag { Name = "Gym" },
                    new Tag { Name = "Travel" },
                    new Tag { Name = "Outdoor" },
                    new Tag { Name = "Home" },

                    // Season Tags
                    new Tag { Name = "Spring" },
                    new Tag { Name = "Summer" },
                    new Tag { Name = "Fall" },
                    new Tag { Name = "Winter" },
                    new Tag { Name = "All Season" },

                    // Brand Tags (Generic)
                    new Tag { Name = "Premium" },
                    new Tag { Name = "Budget-Friendly" },
                    new Tag { Name = "Luxury" },
                    new Tag { Name = "Handmade" },
                    new Tag { Name = "Limited Edition" },
                    new Tag { Name = "New Arrival" },
                    new Tag { Name = "Best Seller" },
                    new Tag { Name = "Sale" }
                };

                foreach (var tag in tags)
                {
                    await _unitOfWork.Tags.AddAsync(tag);
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Successfully seeded {tags.Count} tags.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding tags.");
                throw;
            }
        }
    }
} 