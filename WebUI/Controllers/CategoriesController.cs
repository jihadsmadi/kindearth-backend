using Application.Commands.Categories;
using Application.Queries.Categories;
using Application.DTOs.Categories;
using Core.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all categories (Public - Customers can view)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllCategoriesQuery();
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(
                        result.Value,
                        "Categories retrieved successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to retrieve categories",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving categories",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get category by ID (Public - Customers can view)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetCategoryByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<CategoryDto>.SuccessResult(
                        result.Value,
                        "Category retrieved successfully"));
                }

                return NotFound(ApiResponse.FailureResult(
                    "Category not found",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving the category",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get categories by gender (Public - Customers can view)
        /// </summary>
        [HttpGet("gender/{gender}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByGender(Core.Enums.Gender gender)
        {
            try
            {
                var query = new GetCategoriesByGenderQuery(gender);
                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(
                        result.Value,
                        $"Categories for {gender} retrieved successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to retrieve categories",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving categories",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new category (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            try
            {
                var command = new CreateCategoryCommand(request.Name, request.ImageUrl, request.Gender);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Value },
                        ApiResponse<int>.SuccessResult(
                            result.Value,
                            "Category created successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to create category",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while creating the category",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update an existing category (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateCategoryRequest request)
        {
            try
            {
                var command = new UpdateCategoryCommand(id, request.Name, request.ImageUrl, request.Gender);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse.SuccessResult("Category updated successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to update category",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the category",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a category (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteCategoryCommand(id);
                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    return Ok(ApiResponse.SuccessResult("Category deleted successfully"));
                }

                return BadRequest(ApiResponse.FailureResult(
                    "Failed to delete category",
                    new List<string> { result.Error }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while deleting the category",
                    new List<string> { ex.Message }));
            }
        }
    }
} 