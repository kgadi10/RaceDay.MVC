using System.Net.Http.Json;

namespace RaceDay.MVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string SessionCookieKey = "RaceDayApiCookie";

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("RaceDayAPI");
        }

        private string? GetApiCookie()
        {
            return _httpContextAccessor.HttpContext?
                .Session.GetString(SessionCookieKey);
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod method,
            string endpoint)
        {
            var request = new HttpRequestMessage(method, endpoint);

            var cookie = GetApiCookie();

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie", cookie);
            }

            return request;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            using var request = CreateRequest(HttpMethod.Get, endpoint);
            return await CreateClient().SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data)
        {
            using var request = CreateRequest(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(data);

            return await CreateClient().SendAsync(request);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data)
        {
            using var request = CreateRequest(HttpMethod.Put, endpoint);
            request.Content = JsonContent.Create(data);

            return await CreateClient().SendAsync(request);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            using var request = CreateRequest(HttpMethod.Delete, endpoint);
            return await CreateClient().SendAsync(request);
        }

        public async Task<bool> LoginAsync<T>(T loginDetails)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/Auth/login");

            request.Content = JsonContent.Create(loginDetails);

            using var response = await CreateClient().SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookieHeaders))
            {
                var cookies = cookieHeaders
                    .Select(cookie => cookie.Split(';')[0])
                    .Where(cookie => !string.IsNullOrWhiteSpace(cookie));

                var cookieValue = string.Join("; ", cookies);

                _httpContextAccessor.HttpContext?.Session.SetString(
                    SessionCookieKey, cookieValue);

                return !string.IsNullOrWhiteSpace(cookieValue);
            }

            return false;
        }

        public void ClearApiSession()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(
                SessionCookieKey);
        }
    }
}