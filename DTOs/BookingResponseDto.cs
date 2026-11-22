namespace HotelBookingApi.DTOs
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public string GuestName { get; set; } = null!;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = null!;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
