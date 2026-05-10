namespace SpaceTrackerAPIWebApp.Services
{
    public class NasaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NasaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["NasaApiKey"] ?? "DEMO_KEY";
        }

        public async Task<string> GetApodAsync(string? date = null)
        {
            var url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}";
            if (!string.IsNullOrEmpty(date))
                url += $"&date={date}";
            return await _httpClient.GetStringAsync(url);
        }

        public async Task<string> GetAsteroidsAsync(string startDate, string endDate)
        {
            var url = $"https://api.nasa.gov/neo/rest/v1/feed" +
                      $"?start_date={startDate}&end_date={endDate}" +
                      $"&api_key={_apiKey}";
            return await _httpClient.GetStringAsync(url);
        }
    }
}