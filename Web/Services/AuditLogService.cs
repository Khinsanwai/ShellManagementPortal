using System.Text.Json;
using System.Text;

namespace ShellMgmt.Web.Services;

public class AuditLogService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(IConfiguration configuration, ILogger<AuditLogService> logger)
    {
        _logger = logger;
        _apiBaseUrl = configuration["SMPConfig:Url"] ?? string.Empty;
        _logger.LogInformation("AuditLogService initialized. API URL: {Url}", _apiBaseUrl);

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);
    }

    public async Task LogAsync(string? userId, string? username, string category, string action,
        string result, string? description = null, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            if (string.IsNullOrEmpty(_apiBaseUrl))
            {
                _logger.LogWarning("AuditLogService: API URL is empty, skipping log");
                return;
            }

            // Get access token from the current HTTP context if available
            var token = await GetAccessTokenAsync();

            var logEntry = new
            {
                userId = userId ?? string.Empty,
                username = username ?? string.Empty,
                category,
                action,
                application = "ShellManagementPortal",
                module = "Authentication",
                description = description ?? string.Empty,
                result,
                ipAddress = ipAddress ?? string.Empty,
                userAgent = userAgent ?? string.Empty
            };

            var json = JsonSerializer.Serialize(logEntry);
            _logger.LogInformation("AuditLogService: Sending log to API. Payload: {Payload}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = _apiBaseUrl.Replace("{0}", "userlog/create");
            if (url.Contains("{0}"))
            {
                url = string.Format(_apiBaseUrl, "userlog/create");
            }

            _logger.LogInformation("AuditLogService: POST to {Url}", url);

            // Set auth header if we have a token
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("AuditLogService: Failed to write audit log. Status: {Status}, Body: {Body}",
                    response.StatusCode, responseBody);
            }
            else
            {
                _logger.LogInformation("AuditLogService: Log written successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AuditLogService: Error writing audit log");
        }
    }

    private static async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            // Access token is stored in the cookie by OIDC middleware
            // We can't easily access HttpContext from a singleton service
            // The API endpoint should accept requests without auth for logging
            return await Task.FromResult<string?>(null);
        }
        catch
        {
            return null;
        }
    }
}
