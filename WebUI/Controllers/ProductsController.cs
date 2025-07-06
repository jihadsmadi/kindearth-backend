using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Commands.Products;
using Application.DTOs.Products;
using Application.Queries.Products;
using Core.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all products (Public - Customers can view) - Lightweight version for better performance
        /// </summary>
        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllList()
        {
            try
            {
                var result = await _mediator.Send(new GetAllProductsListQuery());
                return Ok(ApiResponse<IEnumerable<ProductListDto>>.SuccessResult(result, "Products retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving products",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get all products with full details (Public - Customers can view)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _mediator.Send(new GetAllProductsQuery());
                return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResult(result, "Products retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving products",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get product by ID (Public - Customers can view)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetProductByIdQuery(id));
                if (result == null)
                {
                    return NotFound(ApiResponse.FailureResult(
                        "Product not found",
                        new List<string> { $"No product with ID {id}" }));
                }
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving the product",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Create a new product (Vendors and Admins can create)
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var result = await _mediator.Send(new CreateProductCommand(request));
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    ApiResponse<ProductDto>.SuccessResult(result, "Product created successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while creating the product",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update an existing product (Vendors and Admins can update) - Full update
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.FailureResult(
                        "Product ID mismatch",
                        new List<string> { "The ID in the URL does not match the ID in the request body." }));
                }
                var result = await _mediator.Send(new UpdateProductCommand(request));
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product updated successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the product",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update product basic information only (name, description, category)
        /// </summary>
        [HttpPatch("{id}/basic-info")]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBasicInfo(int id, [FromBody] UpdateProductBasicInfoRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.FailureResult(
                        "Product ID mismatch",
                        new List<string> { "The ID in the URL does not match the ID in the request body." }));
                }
                var result = await _mediator.Send(new UpdateProductBasicInfoCommand(request));
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product basic info updated successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the product",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update product images only
        /// </summary>
        [HttpPatch("{id}/images")]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateImages(int id, [FromBody] UpdateProductImagesRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.FailureResult(
                        "Product ID mismatch",
                        new List<string> { "The ID in the URL does not match the ID in the request body." }));
                }
                var result = await _mediator.Send(new UpdateProductImagesCommand(request));
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product images updated successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the product images",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update product stocks only
        /// </summary>
        [HttpPatch("{id}/stocks")]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStocks(int id, [FromBody] UpdateProductStocksRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.FailureResult(
                        "Product ID mismatch",
                        new List<string> { "The ID in the URL does not match the ID in the request body." }));
                }
                var result = await _mediator.Send(new UpdateProductStocksCommand(request));
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product stocks updated successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the product stocks",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update product tags only
        /// </summary>
        [HttpPatch("{id}/tags")]
        [Authorize(Policy = "VendorPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTags(int id, [FromBody] UpdateProductTagsRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.FailureResult(
                        "Product ID mismatch",
                        new List<string> { "The ID in the URL does not match the ID in the request body." }));
                }
                var result = await _mediator.Send(new UpdateProductTagsCommand(request));
                return Ok(ApiResponse<ProductDto>.SuccessResult(result, "Product tags updated successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while updating the product tags",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Delete a product (Admins only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductCommand(id));
                if (!result)
                {
                    return NotFound(ApiResponse.FailureResult(
                        "Product not found or could not be deleted",
                        new List<string> { $"No product with ID {id}" }));
                }
                return Ok(ApiResponse.SuccessResult("Product deleted successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while deleting the product",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Get product stock by product ID (Public - Customers can view)
        /// </summary>
        [HttpGet("{id}/stock")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductStock(int id)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                if (product == null)
                {
                    return NotFound(ApiResponse.FailureResult(
                        "Product not found",
                        new List<string> { $"No product with ID {id}" }));
                }
                return Ok(ApiResponse<IEnumerable<ProductStockDto>>.SuccessResult(product.Stocks, "Product stock retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResult(
                    "An unexpected error occurred while retrieving product stock",
                    new List<string> { ex.Message }));
            }
        }
    }
} 