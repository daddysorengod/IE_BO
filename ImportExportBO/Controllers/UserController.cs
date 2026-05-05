using Application.Users;
using Application.Users.Models;
using ImportExportBO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImportExportBO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.LoginAsync(request, cancellationToken);
                if (result is null)
                {
                    return Unauthorized(new ResponseBase<object>
                    {
                        Code = -1,
                        Message = "Invalid username or password.",
                        Data = null
                    });
                }

                return Ok(new ResponseBase<LoginResponse>
                {
                    Code = result.Id,
                    Message = "Login successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user login input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User login forbidden.");
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.RefreshTokenAsync(request, cancellationToken);
                if (result is null)
                {
                    return Unauthorized(new ResponseBase<object>
                    {
                        Code = -1,
                        Message = "Invalid or expired refresh token.",
                        Data = null
                    });
                }

                return Ok(new ResponseBase<LoginResponse>
                {
                    Code = result.Id,
                    Message = "Refresh token successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid refresh token input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RefreshToken failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("Revoke-Token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.RevokeTokenAsync(request, cancellationToken);
                if (!result.IsRevoked)
                {
                    return NotFound(new ResponseBase<RevokeTokenResponse>
                    {
                        Code = -1,
                        Message = "Refresh token not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<RevokeTokenResponse>
                {
                    Code = result.Id,
                    Message = "Revoke token successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid revoke token input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RevokeToken failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [Authorize]
        [HttpPost("Logout/{userId:long}")]
        public async Task<IActionResult> Logout([FromRoute] long userId, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!long.TryParse(currentUserIdValue, out var currentUserId) || currentUserId != userId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseBase<object>
                    {
                        Code = -1,
                        Message = "You are not allowed to logout this user.",
                        Data = null
                    });
                }

                var result = await _userService.LogoutAsync(userId, cancellationToken);
                if (!result.IsLoggedOut)
                {
                    return NotFound(new ResponseBase<LogoutResponse>
                    {
                        Code = -1,
                        Message = "User not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<LogoutResponse>
                {
                    Code = result.Id,
                    Message = "Logout successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user logout input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("Register-User")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.RegisterAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new ResponseBase<RegisterUserResponse>
                {
                    Code = result.Id,
                    Message = "Registered successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user register input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User register conflict.");
                return Conflict(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RegisterUser failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPut("Update-User")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.UpdateAsync(request, cancellationToken);
                if (!result.IsUpdated)
                {
                    return NotFound(new ResponseBase<UpdateUserResponse>
                    {
                        Code = -1,
                        Message = "User not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<UpdateUserResponse>
                {
                    Code = result.Id,
                    Message = "Updated successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user update input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User update conflict.");
                return Conflict(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpDelete("Delete-User/{userId:long}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.DeleteAsync(userId, cancellationToken);
                if (!result.IsDeleted)
                {
                    return NotFound(new ResponseBase<DeleteUserResponse>
                    {
                        Code = -1,
                        Message = "User not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<DeleteUserResponse>
                {
                    Code = result.Id,
                    Message = "Deleted successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user delete input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUser failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseBase<object>
                {
                    Code = -1,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPut("Activate-User/{userId:long}")]
        public async Task<IActionResult> ActivateUser([FromRoute] long userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.ActivateAsync(userId, cancellationToken);
                if (!result.IsActivated)
                {
                    return NotFound(new ResponseBase<ActivateUserResponse>
                    {
                        Code = -1,
                        Message = "User not found.",
                        Data = result
                    });
                }

                return Ok(new ResponseBase<ActivateUserResponse>
                {
                    Code = result.Id,
                    Message = "Activated successfully.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user activate input.");
                return BadRequest(new ResponseBase<object>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivateUser failed.");
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
