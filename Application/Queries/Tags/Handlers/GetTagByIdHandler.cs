using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Tags;
using Application.Queries.Tags;
using Core.Interfaces;
using MediatR;
using Core.Common;

namespace Application.Queries.Tags.Handlers
{
    public class GetTagByIdHandler : IRequestHandler<GetTagByIdQuery, Result<TagDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTagByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TagDto>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id);
            if (tag == null)
                return Result<TagDto>.Failure("Tag not found");

            var tagDto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return Result<TagDto>.Success(tagDto);
        }
    }
} 