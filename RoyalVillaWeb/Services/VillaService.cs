using RoyalVIlla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services;

public class VillaService : BaseService, IVillaService
{
    private readonly string _villaUrl;
    private const string ApiEndpoint = "/api/villa";
    public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
    {
        _villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI") ?? throw new ArgumentNullException("VillaAPI URL is not configured.");
    }

    public Task<T?> CreateAsync<T>(VillaCreateDTO dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = $"{_villaUrl}{ApiEndpoint}",
            Token = token
        });
    }

    public Task<T?> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{_villaUrl}{ApiEndpoint}/{id}",
            Token = token
        });
    }

    public Task<T?> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{_villaUrl}{ApiEndpoint}",
            Token = token
        });
    }

    public Task<T?> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{_villaUrl}{ApiEndpoint}/{id}",
            Token = token
        });
    }

    public Task<T?> UpdateAsync<T>(VillaUpdateDTO dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = $"{_villaUrl}{ApiEndpoint}/{dto.Id}",
            Token = token
        });
    }
}
