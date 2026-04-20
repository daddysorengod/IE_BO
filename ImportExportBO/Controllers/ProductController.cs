using Application.Products;
using Application.Products.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Mvc;

namespace ImportExportBO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("Get-Product/{productId:long}")]
        public async Task<IActionResult> GetProductById([FromRoute] long productId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.GetByIdAsync(productId, cancellationToken);
                if (result is null)
                {
                    return NotFound(new ResponseBase<object>
                    {
                        Code = -1,
                        Message = "Product not found.",
                        Data = null
                    });
                }

                return Ok(new ResponseBase<GetProductByIdResponse>
                {
                    Code = result.Id,
                    Message = "Get product successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product get-by-id input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProductById failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPost("Insert-Product")]
        public async Task<IActionResult> InsertProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.CreateAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<CreateProductResponse>
                {
                    Code = result.Id,
                    Message = "Created successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InsertProduct failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPut("Update-Product")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.UpdateAsync(request, cancellationToken);
                if (!result.IsUpdated)
                {
                    return NotFound(new ResponseBase<UpdateProductResponse>
                    {
                        Code = -1,
                        Message = "Product not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<UpdateProductResponse>
                {
                    Code = result.Id,
                    Message = "Updated successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product update input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProduct failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpDelete("Delete-Product/{productId:long}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] long productId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.DeleteAsync(productId, cancellationToken);
                if (!result.IsDeleted)
                {
                    return NotFound(new ResponseBase<DeleteProductResponse>
                    {
                        Code = -1,
                        Message = "Product not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<DeleteProductResponse>
                {
                    Code = result.Id,
                    Message = "Deleted successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product delete input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteProduct failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpGet("Search-Product")]
        public async Task<IActionResult> SearchProduct([FromQuery] SearchProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _productService.SearchAsync(request, cancellationToken);
                return Ok(new ResponseBase<SearchProductResponse>
                {
                    Code = result.Total,
                    Message = "Search successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product search input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }
    }
}
