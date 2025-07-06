using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Images)
                .Include(p => p.Stocks)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Images)
                .Include(p => p.Stocks)
                .Include(p => p.ProductTags)
                .ToListAsync();
        }

        public async Task AddAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
        }

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
        }

        public async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
        }

        public async Task<IEnumerable<Product>> GetProductsByVendorIdAsync(Guid vendorProfileId)
        {
            return await _context.Products
                .Where(p => p.VendorProfileId == vendorProfileId)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Stocks)
                .Include(p => p.ProductTags)
                .ToListAsync();
        }
    }
} 