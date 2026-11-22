namespace HotelBookingApi.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public int Floor { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
