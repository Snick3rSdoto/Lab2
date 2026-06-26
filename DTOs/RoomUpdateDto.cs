using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DTOs
{
    public class RoomUpdateDto
    {
        [Required]
        [StringLength(20)]
        public string Number { get; set; } = null!;

        [Required]
        [Range(1, 100)]
        public int Floor { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required]
        public int RoomTypeId { get; set; }
    }
}
