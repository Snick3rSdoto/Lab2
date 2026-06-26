using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DTOs
{
    public class RoomTypeCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 20)]
        public int Capacity { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal PricePerNight { get; set; }
    }
}
