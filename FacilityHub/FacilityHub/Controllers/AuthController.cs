using FacilityHub.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FacilityHub.Controllers;
    [ApiController]
[Route("api/v1/[controller]")]
public class AuthController : Controller
{

    [HttpPost("login")]
    public async Task<IActionResult> Login( LoginDto loginDto)
    {
        return Ok();
    }
    
   
}