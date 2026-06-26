using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Services
{
    public class GuestService
    {
        private readonly HotelContext _dbContext;

        public GuestService(HotelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<GuestResponseDto>> GetAllAsync()
        {
            return await _dbContext.Guests
                .Select(g => new GuestResponseDto
                {
                    Id = g.Id,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    Email = g.Email,
                    Phone = g.Phone
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<GuestResponseDto>> GetByIdAsync(int id)
        {
            var guest = await _dbContext.Guests.FindAsync(id);
            if (guest == null)
            {
                return ServiceResult<GuestResponseDto>.NotFound("Guest not found.");
            }

            return ServiceResult<GuestResponseDto>.Success(new GuestResponseDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone
            });
        }

        public async Task<ServiceResult<GuestResponseDto>> CreateAsync(GuestCreateDto dto)
        {
            if (await _dbContext.Guests.AnyAsync(g => g.Email == dto.Email.Trim()))
            {
                return ServiceResult<GuestResponseDto>.Conflict("Email already exists.");
            }

            var guest = new Guest
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone.Trim()
            };

            _dbContext.Guests.Add(guest);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<GuestResponseDto>.Success(new GuestResponseDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone
            });
        }

        public async Task<ServiceResult<GuestResponseDto>> UpdateAsync(int id, GuestUpdateDto dto)
        {
            var guest = await _dbContext.Guests.FindAsync(id);
            if (guest == null)
            {
                return ServiceResult<GuestResponseDto>.NotFound("Guest not found.");
            }

            if (await _dbContext.Guests.AnyAsync(g => g.Email == dto.Email.Trim() && g.Id != id))
            {
                return ServiceResult<GuestResponseDto>.Conflict("Email already exists.");
            }

            guest.FirstName = dto.FirstName.Trim();
            guest.LastName = dto.LastName.Trim();
            guest.Email = dto.Email.Trim();
            guest.Phone = dto.Phone.Trim();

            await _dbContext.SaveChangesAsync();

            return ServiceResult<GuestResponseDto>.Success(new GuestResponseDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone
            });
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var guest = await _dbContext.Guests.Include(g => g.Bookings).FirstOrDefaultAsync(g => g.Id == id);
            if (guest == null)
            {
                return ServiceResult<bool>.NotFound("Guest not found.");
            }

            if (guest.Bookings.Any(b => b.CheckOut > DateTime.UtcNow))
            {
                return ServiceResult<bool>.Conflict("Cannot delete guest with active bookings.");
            }

            _dbContext.Guests.Remove(guest);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}
