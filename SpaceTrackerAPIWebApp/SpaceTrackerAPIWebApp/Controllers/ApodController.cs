using Microsoft.AspNetCore.Mvc;
using SpaceTrackerAPIWebApp.Services;

namespace SpaceTrackerAPIWebApp.Controllers
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
        public async Task<IActionResult> Get([FromQuery] string? date = null)
        {
            try
            {
                var result = await _nasaService.GetApodAsync(date);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка отримання даних NASA: {ex.Message}");
            }
        }
    }
}  