using Microsoft.AspNetCore.Mvc;
using SpaceTrackerAPIWebApp.Services;

namespace SpaceTrackerAPIWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsteroidsController : ControllerBase
    {
        private readonly NasaService _nasaService;

        public AsteroidsController(NasaService nasaService)
        {
            _nasaService = nasaService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            try
            {
                var result = await _nasaService.GetAsteroidsAsync(startDate, endDate);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка отримання даних про астероїди: {ex.Message}");
            }
        }
    }
}