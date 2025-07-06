using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Tags.Handlers
{
    public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTagHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<bool>.Failure("Tag name is required");

            var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id);
            if (tag == null)
                return Result<bool>.Failure("Tag not found");

            tag.Name = request.Name.Trim();
            await _unitOfWork.Tags.UpdateAsync(tag);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
} 