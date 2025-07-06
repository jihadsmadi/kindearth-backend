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
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public CreateProductHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Create the product
                var product = new Product
                {
                    Name = request.Request.Name,
                    Description = request.Request.Description,
                    CategoryId = request.Request.CategoryId,
                    VendorProfileId = request.Request.VendorProfileId
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                // Add product images
                if (request.Request.ImageUrls?.Any() == true)
                {
                    foreach (var imageUrl in request.Request.ImageUrls)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            Url = imageUrl
                        };
                        await _unitOfWork.ProductImages.AddAsync(productImage);
                    }
                }

                // Add product stocks
                if (request.Request.Stocks?.Any() == true)
                {
                    foreach (var stockRequest in request.Request.Stocks)
                    {
                        var productStock = new ProductStock
                        {
                            ProductId = product.Id,
                            Size = stockRequest.Size,
                            Color = stockRequest.Color,
                            Quantity = stockRequest.Quantity
                        };
                        await _unitOfWork.ProductStocks.AddAsync(productStock);
                    }
                }

                // Add product tags
                if (request.Request.TagIds?.Any() == true)
                {
                    foreach (var tagId in request.Request.TagIds)
                    {
                        var productTag = new ProductTag
                        {
                            ProductId = product.Id,
                            TagId = tagId
                        };
                        await _unitOfWork.ProductTags.AddAsync(productTag);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Get the created product with all related data for mapping
                var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
                return _mappingService.MapToProductDto(createdProduct);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
} 