using Application.DTOs.Tags;
using Core.Common;
using MediatR;

namespace Application.Queries.Tags
{
    public record GetTagByIdQuery(int Id) : IRequest<Result<TagDto>>;
} 