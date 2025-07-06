using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class DataSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(IServiceProvider serviceProvider, ILogger<DataSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedAllAsync()
        {
            try
            {
                _logger.LogInformation("Starting data seeding process...");

                // Seed users and roles first
                await UserSeeder.SeedRolesAndAdminUser(_serviceProvider);

                // Seed categories
                var categorySeeder = _serviceProvider.GetRequiredService<CategorySeeder>();
                await categorySeeder.SeedAsync();

                // Seed tags
                var tagSeeder = _serviceProvider.GetRequiredService<TagSeeder>();
                await tagSeeder.SeedAsync();

                // Seed products
                var productSeeder = _serviceProvider.GetRequiredService<ProductSeeder>();
                await productSeeder.SeedAsync();

                _logger.LogInformation("Data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding.");
                throw;
            }
        }
    }
} 