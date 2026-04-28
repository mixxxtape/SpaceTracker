using Microsoft.AspNetCore.Mvc;
using SpaceTrackerApp.Services;

namespace SpaceTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssController : ControllerBase
    {
        private readonly IssService _issService;

        public IssController(IssService issService)
        {
            _issService = issService;
        }

        // GET api/iss/position
        [HttpGet("position")]
        public async Task<IActionResult> GetPosition()
        {
            var result = await _issService.GetPositionAsync();
            return Ok(result);
        }

        // GET api/iss/pass?lat=50.45&lon=30.52
        [HttpGet("pass")]
        public async Task<IActionResult> GetPassTimes(
            [FromQuery] double lat,
            [FromQuery] double lon)
        {
            var result = await _issService.GetPassTimesAsync(lat, lon);
            return Ok(result);
        }
    }
}