using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Services
{
    public class RoomTypeService
    {
        private readonly HotelContext _dbContext;

        public RoomTypeService(HotelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RoomTypeResponseDto>> GetAllAsync()
        {
            return await _dbContext.RoomTypes
                .Select(rt => new RoomTypeResponseDto
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    Capacity = rt.Capacity,
                    PricePerNight = rt.PricePerNight
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<RoomTypeResponseDto>> GetByIdAsync(int id)
        {
            var roomType = await _dbContext.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                return ServiceResult<RoomTypeResponseDto>.NotFound("Room type not found.");
            }

            return ServiceResult<RoomTypeResponseDto>.Success(new RoomTypeResponseDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Capacity = roomType.Capacity,
                PricePerNight = roomType.PricePerNight
            });
        }

        public async Task<ServiceResult<RoomTypeResponseDto>> CreateAsync(RoomTypeCreateDto dto)
        {
            var roomType = new RoomType
            {
                Name = dto.Name.Trim(),
                Capacity = dto.Capacity,
                PricePerNight = dto.PricePerNight
            };

            _dbContext.RoomTypes.Add(roomType);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<RoomTypeResponseDto>.Success(new RoomTypeResponseDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Capacity = roomType.Capacity,
                PricePerNight = roomType.PricePerNight
            });
        }

        public async Task<ServiceResult<RoomTypeResponseDto>> UpdateAsync(int id, RoomTypeUpdateDto dto)
        {
            var roomType = await _dbContext.RoomTypes.FindAsync(id);
            if (roomType == null)
            {
                return ServiceResult<RoomTypeResponseDto>.NotFound("Room type not found.");
            }

            roomType.Name = dto.Name.Trim();
            roomType.Capacity = dto.Capacity;
            roomType.PricePerNight = dto.PricePerNight;

            await _dbContext.SaveChangesAsync();

            return ServiceResult<RoomTypeResponseDto>.Success(new RoomTypeResponseDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Capacity = roomType.Capacity,
                PricePerNight = roomType.PricePerNight
            });
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var roomType = await _dbContext.RoomTypes.Include(rt => rt.Rooms).FirstOrDefaultAsync(rt => rt.Id == id);
            if (roomType == null)
            {
                return ServiceResult<bool>.NotFound("Room type not found.");
            }

            if (roomType.Rooms.Any())
            {
                return ServiceResult<bool>.Conflict("Cannot delete a room type that has assigned rooms.");
            }

            _dbContext.RoomTypes.Remove(roomType);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }
    }
}
