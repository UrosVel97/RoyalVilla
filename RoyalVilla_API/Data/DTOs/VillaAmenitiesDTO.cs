using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Data.DTOs
{
    public class VillaAmenitiesDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public int VillaId { get; set; }

        public string? VillaName { get; set; }
    }
}
