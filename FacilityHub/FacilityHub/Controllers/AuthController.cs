using FacilityHub.Services;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacilityHub.Controllers;
    [ApiController]
[Route("api/v1/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResult<AuthResponse>>> Login( LoginDto loginDto)
    {
        var res =await _authService.Login(new LoginContext
        {
           Ipaddress  = HttpContext.Connection.RemoteIpAddress?.ToString(),
           LoginDto = loginDto,
           UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
            
        },  HttpContext.RequestAborted);
        
        
        return StatusCode((int)res.StatusCode , res);
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {

        var res =await _authService.Register( registerDto,  HttpContext.RequestAborted);
        
        
        return StatusCode((int)res.StatusCode , res);
    }
}