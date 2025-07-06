using Core.Common;
using Core.Entities;
using Core.Interfaces;
using MediatR;

namespace Application.Commands.Tags.Handlers
{
    public class CreateTagHandler : IRequestHandler<CreateTagCommand, Result<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTagHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<int>.Failure("Tag name is required");

            var tag = new Tag
            {
                Name = request.Name.Trim()
            };

            await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            return Result<int>.Success(tag.Id);
        }
    }
} 