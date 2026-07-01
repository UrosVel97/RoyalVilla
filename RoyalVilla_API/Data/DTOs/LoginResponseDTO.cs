namespace RoyalVilla_API.Data.DTOs
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }

        public UserDTO? UserDTO { get; set; }
    }
}
