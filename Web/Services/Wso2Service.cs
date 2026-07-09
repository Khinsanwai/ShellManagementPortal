using System.Net.Http.Headers;
using System.Text.Json;
using ShellMgmt.Web.Models;

namespace ShellMgmt.Web.Services;

public class Wso2Service(HttpClient httpClient, IConfiguration config, ILogger<Wso2Service> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    private readonly ILogger<Wso2Service> _logger = logger;

    public async Task SessionClear(string refToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _config["WSO2:RevokeEndPoint"]);
        var formData = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", _config["WSO2:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", _config["WSO2:ClientSecret"]!),
            new KeyValuePair<string, string>("token", refToken),
        ]);

        request.Content = formData;
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetRptAsync(string token)
    {
        var tokenEndpoint = _config["WSO2:TokenEndPoint"];
        _logger.LogInformation("WSO2 TokenEndPoint: {Endpoint}", tokenEndpoint);

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

        var formData = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket"),
            new KeyValuePair<string, string>("audience", _config["WSO2:ClientId"]!),
        ]);

        request.Content = formData;

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResponse?.AccessToken;
        }

        _logger.LogWarning("Failed to get RPT. Status: {StatusCode}", response.StatusCode);
        return null;
    }
}
