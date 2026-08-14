using System.Net;
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
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler);
    }

    private void SetAuth(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static async Task ThrowWithErrorBody(HttpResponseMessage response)
    {
        string errorMessage;
        try
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            // Try to parse JSON error response
            if (!string.IsNullOrEmpty(errorBody))
            {
                var doc = System.Text.Json.JsonDocument.Parse(errorBody);
                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    errorMessage = errorProp.GetString() ?? errorBody;
                }
                else if (doc.RootElement.TryGetProperty("title", out var titleProp))
                {
                    errorMessage = titleProp.GetString() ?? errorBody;
                }
                else
                {
                    errorMessage = errorBody;
                }
            }
            else
            {
                errorMessage = response.ReasonPhrase ?? $"HTTP {(int)response.StatusCode}";
            }
        }
        catch
        {
            errorMessage = response.ReasonPhrase ?? $"HTTP {(int)response.StatusCode}";
        }
        throw new HttpRequestException(errorMessage, null, response.StatusCode);
    }

    public async Task<T?> GetAsync<T>(string endpoint, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string endpoint, T data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.PostAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PostAsync(string endpoint, object data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.PostAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.PostAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PutAsync<T>(string endpoint, T data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.PutAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PutAsync(string endpoint, object data, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.PutAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowWithErrorBody(response);
        }
    }

    public async Task<bool> DeleteAsync(string endpoint, string token)
    {
        string url = string.Format(AppConfig.Url, endpoint);
        SetAuth(token);

        var response = await _httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}
