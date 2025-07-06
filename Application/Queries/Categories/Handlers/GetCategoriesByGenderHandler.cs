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
    public class GetCategoriesByGenderHandler : IRequestHandler<GetCategoriesByGenderQuery, Result<IEnumerable<CategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriesByGenderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesByGenderQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            var filteredCategories = allCategories.Where(c => c.Gender == request.Gender);
            
            var categoryDtos = filteredCategories.Select(category => new CategoryDto
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