using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CategorySizeRepository : GenericRepository<CategorySize>, ICategorySizeRepository
    {
        public CategorySizeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CategorySize>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Where(cs => cs.CategoryId == categoryId)
                .ToListAsync();
        }
    }
} 