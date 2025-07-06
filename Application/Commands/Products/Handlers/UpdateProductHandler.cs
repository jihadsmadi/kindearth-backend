using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Services;
using Core.Interfaces;
using MediatR;
using Core.Entities;

namespace Application.Commands.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public UpdateProductHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
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

                // Update basic product information
                existingProduct.Name = request.Request.Name;
                existingProduct.Description = request.Request.Description;
                existingProduct.CategoryId = request.Request.CategoryId;

                await _unitOfWork.Products.UpdateAsync(existingProduct);

                // Update product images (replace all existing images)
                if (existingProduct.Images?.Any() == true)
                {
                    foreach (var image in existingProduct.Images.ToList())
                    {
                        await _unitOfWork.ProductImages.DeleteAsync(image);
                    }
                }

                // Add new images
                if (request.Request.ImageUrls?.Any() == true)
                {
                    foreach (var imageUrl in request.Request.ImageUrls)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = existingProduct.Id,
                            Url = imageUrl
                        };
                        await _unitOfWork.ProductImages.AddAsync(productImage);
                    }
                }

                // Update product stocks
                if (request.Request.Stocks?.Any() == true)
                {
                    // Remove existing stocks
                    if (existingProduct.Stocks?.Any() == true)
                    {
                        foreach (var stock in existingProduct.Stocks.ToList())
                        {
                            await _unitOfWork.ProductStocks.DeleteAsync(stock);
                        }
                    }

                    // Add new stocks
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

                // Update product tags (replace all existing tags)
                if (existingProduct.ProductTags?.Any() == true)
                {
                    foreach (var tag in existingProduct.ProductTags.ToList())
                    {
                        await _unitOfWork.ProductTags.DeleteAsync(tag);
                    }
                }

                // Add new tags
                if (request.Request.TagIds?.Any() == true)
                {
                    foreach (var tagId in request.Request.TagIds)
                    {
                        var productTag = new ProductTag
                        {
                            ProductId = existingProduct.Id,
                            TagId = tagId
                        };
                        await _unitOfWork.ProductTags.AddAsync(productTag);
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