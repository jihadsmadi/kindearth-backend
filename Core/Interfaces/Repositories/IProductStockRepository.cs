using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IProductStockRepository : IRepository<ProductStock>
    {
        Task<IEnumerable<ProductStock>> GetByProductIdAsync(int productId);
    }
} 