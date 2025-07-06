using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        
        private IProductRepository _products;
        private ICategoryRepository _categories;
        private ITagRepository _tags;
        private IProductStockRepository _productStocks;
        private IProductImageRepository _productImages;
        private IProductTagRepository _productTags;
        private ICategorySizeRepository _categorySizes;
        private IVendorProfileRepository _vendorProfiles;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => 
            _products ??= new ProductRepository(_context);

        public ICategoryRepository Categories => 
            _categories ??= new CategoryRepository(_context);

        public ITagRepository Tags => 
            _tags ??= new TagRepository(_context);

        public IProductStockRepository ProductStocks => 
            _productStocks ??= new ProductStockRepository(_context);

        public IProductImageRepository ProductImages => 
            _productImages ??= new ProductImageRepository(_context);

        public IProductTagRepository ProductTags => 
            _productTags ??= new ProductTagRepository(_context);

        public ICategorySizeRepository CategorySizes => 
            _categorySizes ??= new CategorySizeRepository(_context);

        public IVendorProfileRepository VendorProfiles => 
            _vendorProfiles ??= new VendorProfileRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction?.CommitAsync();
            }
            catch
            {
                await _transaction?.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                _transaction?.Dispose();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
} 