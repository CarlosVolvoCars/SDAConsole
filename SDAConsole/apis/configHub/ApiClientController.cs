using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using SDAConsole.interfaces.API;
using SDAConsole.helpers;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SDAConsole.ConfigHub
{
    public class ApiClientController : IApiClient
    {
        private readonly HttpClient _httpClient;
        // private readonly ILogger<ApiClientController> _logger;

        public string BaseUrl { get; set; } = string.Empty;
        public string AuthorizationHeader { get; set; } = string.Empty;
        public string AcceptHeader { get; set; } = "application/json";
        public double TimeOutSecs { get; set; } = 60;

        public ApiClientController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // _logger = logger;
        }

        public async Task<string> GetAsync(string apiUrl)
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthorizationHeader);
                request.Headers.Accept.Clear();

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));
                request.Headers.Add("Contract-Id", "10002");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            });
        }

        public async Task<string> PostJsonAsync(string apiUrl, string messageBodyJson, string contentType = "application/json-patch+json")
        {
            return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthorizationHeader);
                request.Headers.Accept.Clear();

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));
                request.Headers.Add("Contract-Id", "10002");

                request.Content = new StringContent(messageBodyJson, Encoding.UTF8, contentType);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var cleanedString = jsonString.Replace("\"", ""); // TODO: If something changes in the future, this should be fixed
                return cleanedString;

            });
        }


        public async Task DownloadFileAsync(string url, string fileName, bool overwrite, CancellationToken cancellationToken = default)
        {
            await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthorizationHeader);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                using var fileStream = new FileStream(fileName, overwrite ? FileMode.Create : FileMode.CreateNew);
                await response.Content.CopyToAsync(fileStream);

                return true;
            });
        }
    }
}