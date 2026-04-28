namespace SpaceTrackerApp.Services
{
    public class IssService
    {
        private readonly HttpClient _httpClient;

        public IssService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Поточна позиція МКС
        public async Task<string> GetPositionAsync()
        {
            return await _httpClient
                .GetStringAsync(
                    "https://api.wheretheiss.at/v1/satellites/25544");
        }

        // Час прольоту над координатою
        public async Task<string> GetPassTimesAsync(double lat, double lon)
        {
            return await _httpClient
                .GetStringAsync(
                    $"http://api.open-notify.org/iss-pass.json" +
                    $"?lat={lat}&lon={lon}");
        }
    }
}