using Microsoft.AspNetCore.Mvc;
using SpaceTrackerApp.Services;

namespace SpaceTrackerApp.Controllers
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

        // GET api/asteroids?startDate=2024-01-01&endDate=2024-01-07
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            try
            {
                var result = await _nasaService
                    .GetAsteroidsAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Помилка отримання даних про астероїди");
            }
        }
    }
}