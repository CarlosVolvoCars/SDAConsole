namespace SDAConsole.interfaces.API
{
    public interface IApiClient
    {
        string BaseUrl { get; set; }
        string AuthorizationHeader { get; set; }
        string AcceptHeader { get; set; }
        double TimeOutSecs { get; set; }

        Task<string> GetAsync(string apiUrl);
        Task<string> PostJsonAsync(string apiUrl, string messageBodyJson, string contentType = "application/json-patch+json");
        Task DownloadFileAsync(string url, string fileName, bool overwrite, CancellationToken cancellationToken = default);
    }
}