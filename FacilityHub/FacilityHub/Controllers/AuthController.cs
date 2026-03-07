using System.Security.Claims;
using FacilityHub.Core.Entities;
using FacilityHub.helper;
using FacilityHub.Services;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacilityHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResult<AuthResponse>>> Login(LoginDto loginDto)
    {
        var res = await _authService.Login(new LoginContext
        {
            Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            LoginDto = loginDto,
            UserAgent = UserAgentHelper.GetShortUserAgent(HttpContext.Request.Headers["User-Agent"].ToString())
        }, HttpContext.RequestAborted);

        if (res.IsSuccess && res.Data != null)
            if (res.Data.RefreshToken != null)
                SetRefreshToken(res.Data.RefreshToken);


        return StatusCode((int)res.StatusCode, res);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var res = await _authService.Register(registerDto, HttpContext.RequestAborted);


        return StatusCode((int)res.StatusCode, res);
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ServiceResult<UserInfo>>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var me = await _authService.Me(userId, HttpContext.RequestAborted);
        return StatusCode((int)me.StatusCode, me);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ServiceResult<AuthResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest RefreshTokenRequest)
    {
        var res = await _authService.RefreshToken(RefreshTokenRequest.Token,
            Request.Cookies["X-Refresh-Token"] ?? string.Empty,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgentHelper.GetShortUserAgent(HttpContext.Request.Headers["User-Agent"].ToString() ?? string.Empty),
            HttpContext.RequestAborted);
        if (res.IsSuccess && res.Data != null)
            if (res.Data.RefreshToken != null)
                SetRefreshToken(res.Data.RefreshToken);

        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var res = await _authService.ForgotPassword(dto, HttpContext.RequestAborted);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var res = await _authService.ResetPassword(dto, HttpContext.RequestAborted);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var res = await _authService.VerifyEmail(dto, HttpContext.RequestAborted);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var res = await _authService.ChangePassword(userId, dto, HttpContext.RequestAborted);
        return StatusCode((int)res.StatusCode, res);
    }

    private void SetRefreshToken(RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.ExpiresAt,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };
        Response.Cookies.Append("X-Refresh-Token", refreshToken.Token, cookieOptions);
    }
}