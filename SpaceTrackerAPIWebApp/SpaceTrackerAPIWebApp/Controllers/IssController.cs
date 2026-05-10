using Microsoft.AspNetCore.Mvc;
using SpaceTrackerAPIWebApp.Services;

namespace SpaceTrackerAPIWebApp.Controllers
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

        [HttpGet("position")]
        public async Task<IActionResult> GetPosition()
        {
            try
            {
                var result = await _issService.GetPositionAsync();
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка отримання позиції МКС: {ex.Message}");
            }
        }

        [HttpGet("pass")]
        public async Task<IActionResult> GetPassTimes(
            [FromQuery] double lat,
            [FromQuery] double lon)
        {
            try
            {
                var result = await _issService.GetPassTimesAsync(lat, lon);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка отримання часу прольоту: {ex.Message}");
            }
        }
    }
}