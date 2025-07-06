using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IProductTagRepository : IRepository<ProductTag>
    {
        Task<IEnumerable<ProductTag>> GetByProductIdAsync(int productId);
    }
} 