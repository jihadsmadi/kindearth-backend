using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Categories.Handlers
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
            if (category == null)
                return Result<bool>.Failure("Category not found");

            // TODO: Add validation to check if category is used by products before deleting
            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
} 