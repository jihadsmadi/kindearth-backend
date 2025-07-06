using Application.Commands.Tags;
using Application.Queries.Tags;
using Application.DTOs.Tags;
using Core.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all tags (Public - Customers can view)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllTagsQuery();
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<TagDto>>.SuccessResult(
                        result.Value,
                        "Tags retrieved successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to retrieve tags",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving tags",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get tag by ID (Public - Customers can view)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetTagByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<TagDto>.SuccessResult(
                        result.Value,
                        "Tag retrieved successfully"));
                }

                return NotFound(ApiResponse.FailureResult(
                    "Tag not found",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving the tag",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new tag (Vendors and Admins can create)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTagRequest request)
        {
            try
            {
                var command = new CreateTagCommand(request.Name);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Value },
                        ApiResponse<int>.SuccessResult(
                            result.Value,
                            "Tag created successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to create tag",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while creating the tag",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update an existing tag (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateTagRequest request)
        {
            try
            {
                var command = new UpdateTagCommand(id, request.Name);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse.SuccessResult("Tag updated successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to update tag",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the tag",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a tag (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteTagCommand(id);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse.SuccessResult("Tag deleted successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to delete tag",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while deleting the tag",
                    new List<string> { ex.Message }));
            }
        }
    }
} 