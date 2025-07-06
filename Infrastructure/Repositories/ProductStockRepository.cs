using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductStockRepository : GenericRepository<ProductStock>, IProductStockRepository
    {
        public ProductStockRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductStock>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Where(ps => ps.ProductId == productId)
                .ToListAsync();
        }
    }
} 