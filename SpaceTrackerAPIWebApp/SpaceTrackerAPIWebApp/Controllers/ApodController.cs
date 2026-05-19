using Microsoft.AspNetCore.Mvc;
using SpaceTrackerAPIWebApp.Services;

namespace SpaceTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApodController : ControllerBase
    {
        private readonly NasaService _nasaService;

        public ApodController(NasaService nasaService)
        {
            _nasaService = nasaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? date = null,
            [FromQuery] string? start_date = null,
            [FromQuery] string? end_date = null)
        {
            try
            {
                string result;
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    result = await _nasaService.GetApodRangeAsync(start_date, end_date);
                else
                    result = await _nasaService.GetApodAsync(date);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка: {ex.Message}");
            }
        }
    }
}