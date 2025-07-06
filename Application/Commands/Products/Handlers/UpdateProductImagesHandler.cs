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
    public class UpdateProductImagesHandler : IRequestHandler<UpdateProductImagesCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public UpdateProductImagesHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(UpdateProductImagesCommand request, CancellationToken cancellationToken)
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

                // Remove existing images
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