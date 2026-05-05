using Application.Partners;
using Application.Partners.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImportExportBO.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly ILogger<PartnerController> _logger;

        public PartnerController(IPartnerService partnerService, ILogger<PartnerController> logger)
        {
            _partnerService = partnerService;
            _logger = logger;
        }

        [HttpGet("Get-Partner/{partnerId:long}")]
        public async Task<IActionResult> GetById([FromRoute] long partnerId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _partnerService.GetByIdAsync(partnerId, cancellationToken);
                if (result is null)
                {
                    return NotFound(new ResponseBase<object> { Code = -1, Message = "Partner not found.", Data = null });
                }

                return Ok(new ResponseBase<PartnerResponse> { Code = result.Id, Message = "Get partner successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid partner get input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get partner failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpGet("Search-Partner")]
        public async Task<IActionResult> Search([FromQuery] SearchPartnerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _partnerService.SearchAsync(request, cancellationToken);
                return Ok(new ResponseBase<SearchPartnerResponse> { Code = result.Total, Message = "Search partners successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid partner search input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search partners failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPost("Insert-Partner")]
        public async Task<IActionResult> Create([FromBody] CreatePartnerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _partnerService.CreateAsync(request, GetCurrentUserId(), cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<CreatePartnerResponse> { Code = result.Id, Message = "Created partner successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid partner create input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Partner create conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create partner failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPut("Update-Partner/{partnerId:long}")]
        public async Task<IActionResult> Update([FromRoute] long partnerId, [FromBody] UpdatePartnerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                request.Id = partnerId;
                var result = await _partnerService.UpdateAsync(request, GetCurrentUserId(), cancellationToken);
                if (!result.IsUpdated)
                {
                    return NotFound(new ResponseBase<UpdatePartnerResponse> { Code = -1, Message = "Partner not found.", Data = result });
                }

                return Ok(new ResponseBase<UpdatePartnerResponse> { Code = result.Id, Message = "Updated partner successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid partner update input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Partner update conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update partner failed.");
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
