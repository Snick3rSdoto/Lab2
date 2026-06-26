using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        public int GuestId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }
    }
}
