using RoyalVIlla.DTO;

namespace RoyalVillaWeb.Services.IServices;

public interface IAuthService
{
    Task<T?> LoginAsync<T>(LoginRequestDTO dto);
    Task<T?> RegisterAsync<T>(RegistrationRequestDTO dto);
}
