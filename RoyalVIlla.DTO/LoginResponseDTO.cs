using RoyalVIlla.DTO;

namespace RoyalVIlla.DTO;

public class LoginResponseDTO
{
    public string? Token { get; set; }

    public UserDTO? UserDTO { get; set; }
}
