using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Services;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Products.Handlers
{
    public class UpdateProductBasicInfoHandler : IRequestHandler<UpdateProductBasicInfoCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public UpdateProductBasicInfoHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(UpdateProductBasicInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get existing product
                var existingProduct = await _unitOfWork.Products.GetByIdAsync(request.Request.Id);
                if (existingProduct == null)
                {
                    throw new InvalidOperationException($"Product with ID {request.Request.Id} not found");
                }

                // Update only basic information
                if (!string.IsNullOrEmpty(request.Request.Name))
                    existingProduct.Name = request.Request.Name;
                
                if (!string.IsNullOrEmpty(request.Request.Description))
                    existingProduct.Description = request.Request.Description;
                
                if (request.Request.CategoryId > 0)
                    existingProduct.CategoryId = request.Request.CategoryId;

                await _unitOfWork.Products.UpdateAsync(existingProduct);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Return the updated product DTO
                return _mappingService.MapToProductDto(existingProduct);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
} 