using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductTagRepository : GenericRepository<ProductTag>, IProductTagRepository
    {
        public ProductTagRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductTags
                .Where(pt => pt.ProductId == productId)
                .ToListAsync();
        }
    }
} 