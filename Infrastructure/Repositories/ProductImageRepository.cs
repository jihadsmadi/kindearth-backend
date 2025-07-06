using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }
    }
} 