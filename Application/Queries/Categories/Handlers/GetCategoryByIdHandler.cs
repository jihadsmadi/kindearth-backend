using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Categories;
using Application.Queries.Categories;
using Core.Interfaces;
using MediatR;
using Core.Common;

namespace Application.Queries.Categories.Handlers
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category == null)
                return Result<CategoryDto>.Failure("Category not found");

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
                Gender = category.Gender
            };

            return Result<CategoryDto>.Success(categoryDto);
        }
    }
} 