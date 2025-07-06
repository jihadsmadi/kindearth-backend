using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Categories.Handlers
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<bool>.Failure("Category name is required");

            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category == null)
                return Result<bool>.Failure("Category not found");

            category.Name = request.Name.Trim();
            category.ImageUrl = request.ImageUrl;
            category.Gender = request.Gender;

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
} 