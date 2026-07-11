using RoyalVIlla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services;

public class VillaService : BaseService, IVillaService
{
    private const string ApiEndpoint = "/api/villa";

    public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
    {
    }

    public Task<T?> CreateAsync<T>(VillaCreateDTO dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = $"{ApiEndpoint}",
            Token = token
        });
    }

    public Task<T?> DeleteAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{ApiEndpoint}/{id}",
            Token = token
        });
    }

    public Task<T?> GetAllAsync<T>(string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{ApiEndpoint}",
            Token = token
        });
    }

    public Task<T?> GetAsync<T>(int id, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{ApiEndpoint}/{id}",
            Token = token
        });
    }

    public Task<T?> UpdateAsync<T>(VillaUpdateDTO dto, string token)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = $"{ApiEndpoint}/{dto.Id}",
            Token = token
        });
    }
}
