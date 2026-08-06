using RoyalVIlla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services;

public class VillaService : BaseService, IVillaService
{
    private const string ApiEndpoint = "/api/v1/villa";

    public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        : base(httpClient, httpContextAccessor)
    {
    }

    public Task<T?> CreateAsync<T>(VillaCreateDTO dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = $"{ApiEndpoint}"
        });
    }

    public Task<T?> DeleteAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{ApiEndpoint}/{id}"
        });
    }

    public Task<T?> GetAllAsync<T>()
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{ApiEndpoint}"
        });
    }

    public Task<T?> GetAsync<T>(int id)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{ApiEndpoint}/{id}"
        });
    }

    public Task<T?> UpdateAsync<T>(VillaUpdateDTO dto)
    {
        return SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.PUT,
            Data = dto,
            Url = $"{ApiEndpoint}/{dto.Id}"
        });
    }
}
