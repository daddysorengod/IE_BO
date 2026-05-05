using Application.Allcodes;
using Application.Allcodes.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImportExportBO.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AllcodeController : ControllerBase
    {
        private readonly IAllcodeService _allcodeService;
        private readonly ILogger<AllcodeController> _logger;

        public AllcodeController(IAllcodeService allcodeService, ILogger<AllcodeController> logger)
        {
            _allcodeService = allcodeService;
            _logger = logger;
        }

        [HttpGet("Search-Allcode")]
        public async Task<IActionResult> Search([FromQuery] SearchAllcodeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _allcodeService.SearchAsync(request, cancellationToken);
                return Ok(new ResponseBase<SearchAllcodeResponse>
                {
                    Code = result.Total,
                    Message = "Search allcodes successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid allcode search input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search allcode failed.");
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
