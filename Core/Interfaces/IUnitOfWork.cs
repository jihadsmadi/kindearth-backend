using System;
using System.Threading.Tasks;
using Core.Interfaces.Repositories;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ITagRepository Tags { get; }
        IProductStockRepository ProductStocks { get; }
        IProductImageRepository ProductImages { get; }
        IProductTagRepository ProductTags { get; }
        ICategorySizeRepository CategorySizes { get; }
        IVendorProfileRepository VendorProfiles { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 