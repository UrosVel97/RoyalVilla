using System.ComponentModel.DataAnnotations;

namespace RoyalVIlla.DTO
{
    public class VillaAmenitiesDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public int VillaId { get; set; }

        public string? VillaName { get; set; }
    }
}
