using System.ComponentModel.DataAnnotations;

namespace HotelBookingApi.DTOs
{
    public class GuestCreateDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Phone { get; set; } = null!;
    }
}
