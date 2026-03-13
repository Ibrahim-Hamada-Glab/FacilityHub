using System.Security.Claims;
using FacilityHub.Services.Dtos;
using FacilityHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FacilityHub.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FacilityController : Controller
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFacilities()
    {
        var result = await _facilityService.GetAllFacilitiesAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFacilityById(Guid id)
    {
        var result = await _facilityService.GetFacilityByIdAsync(id);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityDto dto)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value; 
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Message = "User is not authenticated." });
        }
        var result = await _facilityService.CreateFacilityAsync(dto , userId, HttpContext.RequestAborted);
        return StatusCode((int)result.StatusCode, result);
    }
     
}