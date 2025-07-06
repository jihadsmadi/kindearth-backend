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
    public class UpdateProductTagsHandler : IRequestHandler<UpdateProductTagsCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public UpdateProductTagsHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(UpdateProductTagsCommand request, CancellationToken cancellationToken)
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

                // Remove existing tags
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