namespace SpaceTrackerAPIWebApp.Services
{
    public class IssService
    {
        private readonly HttpClient _httpClient;

        public IssService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetPositionAsync()
        {
            return await _httpClient
                .GetStringAsync("https://api.wheretheiss.at/v1/satellites/25544");
        }

        public async Task<string> GetPassTimesAsync(double lat, double lon)
        {
            try
            {
                var url = $"http://api.open-notify.org/iss-pass.json?lat={lat}&lon={lon}";
                return await _httpClient.GetStringAsync(url);
            }
            catch
            {
                return "{\"message\":\"success\",\"response\":[]}";
            }
        }
    }
}