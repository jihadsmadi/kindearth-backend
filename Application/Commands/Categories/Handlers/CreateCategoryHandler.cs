using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Categories.Handlers
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<int>.Failure("Category name is required");

            var category = new Category
            {
                Name = request.Name.Trim(),
                ImageUrl = request.ImageUrl,
                Gender = request.Gender
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result<int>.Success(category.Id);
        }
    }
} 