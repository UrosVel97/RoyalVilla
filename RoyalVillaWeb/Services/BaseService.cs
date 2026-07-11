using RoyalVIlla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;
using System.Text.Json;

namespace RoyalVillaWeb.Services;

public class BaseService : IBaseService
{
    private IHttpClientFactory _httpClient { get; set; }
    public ApiResponse<object> ResponseModel { get; set; }

    private static readonly JsonSerializerOptions JsonOptions = new()
    { 
        PropertyNameCaseInsensitive = true
    };

    public BaseService(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        ResponseModel = new();
    }

    public async Task<T?> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = _httpClient.CreateClient("RoyalVillaAPI");
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri(apiRequest.Url!, UriKind.Relative),
                Method = GetHttpMethod(apiRequest.ApiType)
            };

            if(apiRequest.Data != null)
            {
                message.Content = JsonContent.Create(apiRequest.Data, options: JsonOptions);
            }

            var apiResponse =await client.SendAsync(message);

            return await apiResponse.Content.ReadFromJsonAsync<T>(JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error occurred while sending the API request: " + ex.Message);

            return default!;
        }
    }

    private static HttpMethod GetHttpMethod(SD.ApiType apiRequest)
    {
        return apiRequest switch
        {
            SD.ApiType.POST => HttpMethod.Post,
            SD.ApiType.PUT => HttpMethod.Put,
            SD.ApiType.DELETE => HttpMethod.Delete,
            _ => HttpMethod.Get
        };
    }
}
