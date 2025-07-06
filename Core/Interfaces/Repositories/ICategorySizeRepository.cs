using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface ICategorySizeRepository : IRepository<CategorySize>
    {
        Task<IEnumerable<CategorySize>> GetByCategoryIdAsync(int categoryId);
    }
} 