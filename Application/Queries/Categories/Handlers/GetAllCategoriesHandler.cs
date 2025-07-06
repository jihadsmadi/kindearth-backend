using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Categories;
using Application.Queries.Categories;
using Core.Interfaces;
using MediatR;
using Core.Common;

namespace Application.Queries.Categories.Handlers
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCategoriesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDtos = categories.Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                Gender = category.Gender
            });

            return Result<IEnumerable<CategoryDto>>.Success(categoryDtos);
        }
    }
} 