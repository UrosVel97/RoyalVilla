using RoyalVIlla.DTO;
using RoyalVillaWeb.Models;
using RoyalVillaWeb.Services.IServices;

namespace RoyalVillaWeb.Services;

public class AuthService : BaseService, IAuthService
{
    private const string ApiEndpoint = "/api/auth";

    public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
    {
        
    }

    public async Task<T?> LoginAsync<T>(LoginRequestDTO dto)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = $"{ApiEndpoint}/login"
        });
    }

    public async Task<T?> RegisterAsync<T>(RegistrationRequestDTO dto)
    {
        return await SendAsync<T>(new ApiRequest()
        {
            ApiType = SD.ApiType.POST,
            Data = dto,
            Url = $"{ApiEndpoint}/register"
        });
    }
}
