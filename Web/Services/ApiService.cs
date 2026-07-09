using System.Net.Http.Headers;
using System.Net.Http.Json;
using ShellMgmt.Web.Constants;

namespace ShellMgmt.Web.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);
    }

    public async Task<T?> GetAsync<T>(string endpoint, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string endpoint, T data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.PostAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, T data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.PutAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<bool> DeleteAsync(string endpoint, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}
