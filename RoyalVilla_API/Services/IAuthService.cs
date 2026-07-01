using RoyalVilla_API.Data.DTOs;

namespace RoyalVilla_API.Services
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegistrationRequestDTO requestDTO);

        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO requestDTO);

        Task<bool> IsEmailExistsAsync(string email);
    }
}
