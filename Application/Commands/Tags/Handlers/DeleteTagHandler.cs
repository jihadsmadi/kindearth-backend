using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Tags.Handlers
{
    public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTagHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id);
            if (tag == null)
                return Result<bool>.Failure("Tag not found");

            // TODO: Add validation to check if tag is used by products before deleting
            await _unitOfWork.Tags.DeleteAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<bool>.Success(true);
        }
    }
} 