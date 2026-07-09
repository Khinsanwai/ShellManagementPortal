using System.Net.Http.Headers;
using System.Net.Http.Json;
using ShellMgmt.Domain.TenantModels;
using ShellMgmt.Web.Constants;

namespace ShellMgmt.Web.Services;

public class TenantService(HttpClient httpClient, ILogger<TenantService> logger)
{
    private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });
    private readonly ILogger<TenantService> _logger = logger;

    public async Task<TenantDto?> GetTenantAsync(string name, string token)
    {
        try
        {
            string url = string.Format(AppConfig.Url, $"tenant/getbyname/{name}");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TenantDto>();
            }

            _logger.LogWarning("StatusCode {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tenant");
            throw;
        }
    }
}
