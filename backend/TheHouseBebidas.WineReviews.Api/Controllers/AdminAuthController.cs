using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheHouseBebidas.WineReviews.Application.DTOs.Auth;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;

namespace TheHouseBebidas.WineReviews.Api.Controllers;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly IAdminAuthService _adminAuthService;

    public AdminAuthController(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AdminLoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AdminLoginResponseDto>> LoginAsync(
        [FromBody] AdminLoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var response = await _adminAuthService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }
}
