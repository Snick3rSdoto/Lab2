using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Services
{
    public class BookingService
    {
        private readonly HotelContext _dbContext;

        public BookingService(HotelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            return await _dbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    GuestId = b.GuestId,
                    GuestName = $"{b.Guest.FirstName} {b.Guest.LastName}",
                    RoomId = b.RoomId,
                    RoomNumber = b.Room.Number,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    TotalPrice = b.TotalPrice
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<BookingResponseDto>> GetByIdAsync(int id)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return ServiceResult<BookingResponseDto>.NotFound("Booking not found.");
            }

            return ServiceResult<BookingResponseDto>.Success(MapToDto(booking));
        }

        public async Task<ServiceResult<BookingResponseDto>> CreateAsync(BookingCreateDto dto)
        {
            if (dto.CheckOut <= dto.CheckIn)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Check-out must be later than check-in.");
            }

            var room = await _dbContext.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room == null)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Room does not exist.");
            }

            if (!room.IsAvailable)
            {
                return ServiceResult<BookingResponseDto>.Conflict("Room is not available for booking.");
            }

            var guest = await _dbContext.Guests.FindAsync(dto.GuestId);
            if (guest == null)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Guest does not exist.");
            }

            if (await HasOverlapAsync(dto.RoomId, dto.CheckIn, dto.CheckOut))
            {
                return ServiceResult<BookingResponseDto>.Conflict("The room is already booked for the selected period.");
            }

            var nights = (dto.CheckOut.Date - dto.CheckIn.Date).Days;
            if (nights <= 0)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Booking must span at least one night.");
            }

            var booking = new Booking
            {
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalPrice = nights * room.RoomType.PricePerNight
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();

            var created = await _dbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstAsync(b => b.Id == booking.Id);

            return ServiceResult<BookingResponseDto>.Success(MapToDto(created));
        }

        public async Task<ServiceResult<BookingResponseDto>> UpdateAsync(int id, BookingUpdateDto dto)
        {
            if (dto.CheckOut <= dto.CheckIn)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Check-out must be later than check-in.");
            }

            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return ServiceResult<BookingResponseDto>.NotFound("Booking not found.");
            }

            var room = await _dbContext.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room == null)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Room does not exist.");
            }

            var guest = await _dbContext.Guests.FindAsync(dto.GuestId);
            if (guest == null)
            {
                return ServiceResult<BookingResponseDto>.BadRequest("Guest does not exist.");
            }

            if (await HasOverlapAsync(dto.RoomId, dto.CheckIn, dto.CheckOut, id))
            {
                return ServiceResult<BookingResponseDto>.Conflict("The room is already booked for the selected period.");
            }

            booking.GuestId = dto.GuestId;
            booking.RoomId = dto.RoomId;
            booking.CheckIn = dto.CheckIn;
            booking.CheckOut = dto.CheckOut;
            booking.TotalPrice = (dto.CheckOut.Date - dto.CheckIn.Date).Days * room.RoomType.PricePerNight;

            await _dbContext.SaveChangesAsync();

            var updated = await _dbContext.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstAsync(b => b.Id == id);

            return ServiceResult<BookingResponseDto>.Success(MapToDto(updated));
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return ServiceResult<bool>.NotFound("Booking not found.");
            }

            _dbContext.Bookings.Remove(booking);
            await _dbContext.SaveChangesAsync();
            return ServiceResult<bool>.Success(true);
        }

        private async Task<bool> HasOverlapAsync(int roomId, DateTime checkIn, DateTime checkOut, int? currentBookingId = null)
        {
            return await _dbContext.Bookings
                .Where(b => b.RoomId == roomId && (!currentBookingId.HasValue || b.Id != currentBookingId.Value))
                .AnyAsync(b => b.CheckIn < checkOut && checkIn < b.CheckOut);
        }

        private BookingResponseDto MapToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                GuestId = booking.GuestId,
                GuestName = $"{booking.Guest.FirstName} {booking.Guest.LastName}",
                RoomId = booking.RoomId,
                RoomNumber = booking.Room.Number,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.TotalPrice
            };
        }
    }
}
