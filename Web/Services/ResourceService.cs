using System.Net.Http.Headers;
using System.Net.Http.Json;
using SharedKernel.Domain;
using ShellMgmt.Domain.ResourceModels;
using ShellMgmt.Web.Constants;

namespace ShellMgmt.Web.Services;

public class ResourceService
{
    private readonly HttpClient _httpClient;

    public ResourceService(HttpClient httpClient)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        _httpClient = new HttpClient(handler);
    }

    public async Task<List<ResourceDto>?> GetResourceAsync(string token)
    {
        try
        {
            string url = string.Format(AppConfig.Url, "resource/get");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var pagedResult = await response.Content.ReadFromJsonAsync<PagedList<ResourceDto>>();
                return pagedResult?.Items?.ToList();
            }

            Console.WriteLine($"Failed to fetch resources. Status Code: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
}
