namespace HotelBookingApi.DTOs
{
    public class RoomTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
