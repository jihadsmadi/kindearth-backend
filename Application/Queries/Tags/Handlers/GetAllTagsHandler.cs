using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Tags;
using Application.Queries.Tags;
using Core.Interfaces;
using MediatR;
using Core.Common;

namespace Application.Queries.Tags.Handlers
{
    public class GetAllTagsHandler : IRequestHandler<GetAllTagsQuery, Result<IEnumerable<TagDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTagsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<TagDto>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            var tagDtos = tags.Select(tag => new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            });

            return Result<IEnumerable<TagDto>>.Success(tagDtos);
        }
    }
} 