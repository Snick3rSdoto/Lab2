namespace HotelBookingApi.DTOs
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int Floor { get; set; }
        public bool IsAvailable { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = null!;
        public decimal PricePerNight { get; set; }
    }
}
