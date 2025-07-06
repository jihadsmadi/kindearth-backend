using Core.Entities;

namespace Core.Interfaces.Repositories
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
    }
} 