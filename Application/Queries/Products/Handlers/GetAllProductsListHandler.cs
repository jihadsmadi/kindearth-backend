using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Products;
using Application.Queries.Products;
using Application.Services;
using Core.Interfaces;
using MediatR;

namespace Application.Queries.Products.Handlers
{
    public class GetAllProductsListHandler : IRequestHandler<GetAllProductsListQuery, List<ProductListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductMappingService _mappingService;

        public GetAllProductsListHandler(IUnitOfWork unitOfWork, IProductMappingService mappingService)
        {
            _unitOfWork = unitOfWork;
            _mappingService = mappingService;
        }

        public async Task<List<ProductListDto>> Handle(GetAllProductsListQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mappingService.MapToProductListDtoList(products);
        }
    }
} 