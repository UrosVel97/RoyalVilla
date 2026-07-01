using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Data.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string? Email { get; set; }


        public string? Name { get; set; }

        
        public string? Role { get; set; } 

    }
}
