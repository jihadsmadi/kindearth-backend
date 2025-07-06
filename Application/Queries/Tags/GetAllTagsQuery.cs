using Application.DTOs.Tags;
using Core.Common;
using MediatR;

namespace Application.Queries.Tags
{
    public record GetAllTagsQuery : IRequest<Result<IEnumerable<TagDto>>>;
} 