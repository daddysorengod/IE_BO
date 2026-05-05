using Application.Packages;
using Application.Packages.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImportExportBO.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IPackageService packageService, ILogger<PackageController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet("Search-Package")]
        public async Task<IActionResult> Search([FromQuery] SearchPackageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _packageService.SearchAsync(request, cancellationToken);
                return Ok(new ResponseBase<SearchPackageResponse> { Code = result.Total, Message = "Search packages successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid package search input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search packages failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpGet("Get-Package/{packageId:long}")]
        public async Task<IActionResult> GetById([FromRoute] long packageId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _packageService.GetByIdAsync(packageId, cancellationToken);
                if (result is null)
                {
                    return NotFound(new ResponseBase<object> { Code = -1, Message = "Package not found.", Data = null });
                }

                return Ok(new ResponseBase<PackageResponse> { Code = result.Id, Message = "Get package successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid package get input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get package failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPost("Insert-Package")]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _packageService.CreateAsync(request, GetCurrentUserId(), cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<CreatePackageResponse> { Code = result.Id, Message = "Created package successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid package create input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Package create conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create package failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPut("Update-Package/{packageId:long}")]
        public async Task<IActionResult> Update([FromRoute] long packageId, [FromBody] UpdatePackageRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _packageService.UpdateAsync(packageId, request, GetCurrentUserId(), cancellationToken);
                if (!result.IsUpdated)
                {
                    return NotFound(new ResponseBase<UpdatePackageResponse> { Code = -1, Message = "Package not found.", Data = result });
                }

                return Ok(new ResponseBase<UpdatePackageResponse> { Code = result.Id, Message = "Updated package successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid package update input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Package update conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update package failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpDelete("Delete-Package/{packageId:long}")]
        public async Task<IActionResult> Delete([FromRoute] long packageId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _packageService.DeleteAsync(packageId, GetCurrentUserId(), cancellationToken);
                if (!result.IsDeleted)
                {
                    return NotFound(new ResponseBase<DeletePackageResponse> { Code = -1, Message = "Package not found.", Data = result });
                }

                return Ok(new ResponseBase<DeletePackageResponse> { Code = result.Id, Message = "Deleted package successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid package delete input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete package failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        private long GetCurrentUserId()
        {
            var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(currentUserIdValue, out var currentUserId) || currentUserId <= 0)
            {
                throw new ArgumentException("Current user id is invalid.");
            }

            return currentUserId;
        }
    }
}
