using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Services;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Products.Handlers
{
    public class UpdateProductStocksHandler : IRequestHandler<UpdateProductStocksCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public UpdateProductStocksHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(UpdateProductStocksCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get existing product with all related data
                var existingProduct = await _unitOfWork.Products.GetByIdAsync(request.Request.Id);
                if (existingProduct == null)
                {
                    throw new InvalidOperationException($"Product with ID {request.Request.Id} not found");
                }

                // Remove existing stocks
                if (existingProduct.Stocks?.Any() == true)
                {
                    foreach (var stock in existingProduct.Stocks.ToList())
                    {
                        await _unitOfWork.ProductStocks.DeleteAsync(stock);
                    }
                }

                // Add new stocks
                if (request.Request.Stocks?.Any() == true)
                {
                    foreach (var stockRequest in request.Request.Stocks)
                    {
                        var productStock = new ProductStock
                        {
                            ProductId = existingProduct.Id,
                            Size = stockRequest.Size,
                            Color = stockRequest.Color,
                            Quantity = stockRequest.Quantity
                        };
                        await _unitOfWork.ProductStocks.AddAsync(productStock);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Get the updated product with all related data for mapping
                var updatedProduct = await _unitOfWork.Products.GetByIdAsync(existingProduct.Id);
                return _mappingService.MapToProductDto(updatedProduct);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
} 