using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Products;
using Application.Queries.Products;
using Application.Services;
using Core.Interfaces;
using MediatR;
using System.Linq;

namespace Application.Queries.Products.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public GetProductByIdHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
            
            if (product == null)
                return null;

            return _mappingService.MapToProductDto(product);
        }
    }
} 