using Microsoft.AspNetCore.Mvc;
using SpaceTrackerApp.Services;

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

        // GET api/apod
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string date = null)
        {
            try
            {
                var result = await _nasaService.GetApodAsync(date);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Помилка отримання даних NASA");
            }
        }
    }
}