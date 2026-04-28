namespace SpaceTrackerApp.Services
{
    public class NasaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "ТВІЙ_КЛЮЧ_NASA";

        public NasaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Фото дня
        public async Task<string> GetApodAsync(string date = null)
        {
            var url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}";
            if (date != null) url += $"&date={date}";
            return await _httpClient.GetStringAsync(url);
        }

        // Астероїди
        public async Task<string> GetAsteroidsAsync(
            string startDate, string endDate)
        {
            var url = $"https://api.nasa.gov/neo/rest/v1/feed" +
                      $"?start_date={startDate}&end_date={endDate}" +
                      $"&api_key={_apiKey}";
            return await _httpClient.GetStringAsync(url);
        }
    }
}