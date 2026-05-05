using Application.Contracts;
using Application.Contracts.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImportExportBO.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractController> _logger;

        public ContractController(IContractService contractService, ILogger<ContractController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        [HttpGet("Search-Contract")]
        public async Task<IActionResult> Search([FromQuery] SearchContractRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _contractService.SearchAsync(request, cancellationToken);
                return Ok(new ResponseBase<SearchContractResponse> { Code = result.Total, Message = "Search contracts successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid contract search input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search contracts failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpGet("Get-Contract/{contractId:long}")]
        public async Task<IActionResult> GetDetail([FromRoute] long contractId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _contractService.GetDetailAsync(contractId, cancellationToken);
                if (result is null)
                {
                    return NotFound(new ResponseBase<object> { Code = -1, Message = "Contract not found.", Data = null });
                }

                return Ok(new ResponseBase<ContractDetailResponse> { Code = result.Id, Message = "Get contract successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid contract get input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get contract failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPost("Insert-Contract")]
        public async Task<IActionResult> CreateExport([FromBody] CreateContractRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _contractService.CreateAsync(request, GetCurrentUserId(), cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<CreateContractResponse> { Code = result.Id, Message = "Created export contract successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid export contract create input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Export contract create conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Export contract create related entity not found.");
                return NotFound(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create export contract failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPost("Insert-Import-Contract")]
        public async Task<IActionResult> CreateImport([FromBody] CreateImportContractRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _contractService.CreateImportAsync(request, GetCurrentUserId(), cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<CreateContractResponse> { Code = result.Id, Message = "Created import contract successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid import contract create input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Import contract create conflict.");
                return Conflict(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Import contract create related entity not found.");
                return NotFound(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create import contract failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object> { Code = -1, Message = "Internal server error.", Data = null });
            }
        }

        [HttpPost("Insert-Contract-Attachment/{contractId:long}")]
        public async Task<IActionResult> AddAttachment([FromRoute] long contractId, [FromBody] CreateContractAttachmentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _contractService.AddAttachmentAsync(contractId, request, GetCurrentUserId(), cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<AddContractAttachmentResponse> { Code = result.Id, Message = "Added contract attachment successfully.", Data = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid contract attachment input.");
                return BadRequest(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Contract not found for attachment create.");
                return NotFound(new ResponseBase<object> { Code = -1, Message = ex.Message, Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add contract attachment failed.");
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
