using HotelBookingApi.Data;
using HotelBookingApi.DTOs;
using HotelBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApi.Services
{
    public class RoomService
    {
        private readonly HotelContext _dbContext;

        public RoomService(HotelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            return await _dbContext.Rooms
                .Include(r => r.RoomType)
                .Select(r => new RoomResponseDto
                {
                    Id = r.Id,
                    Number = r.Number,
                    Floor = r.Floor,
                    IsAvailable = r.IsAvailable,
                    RoomTypeId = r.RoomTypeId,
                    RoomTypeName = r.RoomType.Name,
                    PricePerNight = r.RoomType.PricePerNight
                })
                .ToListAsync();
        }

        public async Task<ServiceResult<RoomResponseDto>> GetByIdAsync(int id)
        {
            var room = await _dbContext.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return ServiceResult<RoomResponseDto>.NotFound("Room not found.");
            }

            return ServiceResult<RoomResponseDto>.Success(new RoomResponseDto
            {
                Id = room.Id,
                Number = room.Number,
                Floor = room.Floor,
                IsAvailable = room.IsAvailable,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType.Name,
                PricePerNight = room.RoomType.PricePerNight
            });
        }

        public async Task<ServiceResult<RoomResponseDto>> CreateAsync(RoomCreateDto dto)
        {
            if (dto.Floor <= 0)
            {
                return ServiceResult<RoomResponseDto>.BadRequest("Floor must be greater than zero.");
            }

            if (await _dbContext.Rooms.AnyAsync(r => r.Number == dto.Number.Trim()))
            {
                return ServiceResult<RoomResponseDto>.Conflict("Room number already exists.");
            }

            var roomType = await _dbContext.RoomTypes.FindAsync(dto.RoomTypeId);
            if (roomType == null)
            {
                return ServiceResult<RoomResponseDto>.BadRequest("Room type does not exist.");
            }

            var room = new Room
            {
                Number = dto.Number.Trim(),
                Floor = dto.Floor,
                IsAvailable = dto.IsAvailable,
                RoomTypeId = dto.RoomTypeId
            };

            _dbContext.Rooms.Add(room);
            await _dbContext.SaveChangesAsync();

            return ServiceResult<RoomResponseDto>.Success(new RoomResponseDto
            {
                Id = room.Id,
                Number = room.Number,
                Floor = room.Floor,
                IsAvailable = room.IsAvailable,
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.PricePerNight
            });
        }

        public async Task<ServiceResult<RoomResponseDto>> UpdateAsync(int id, RoomUpdateDto dto)
        {
            if (dto.Floor <= 0)
            {
                return ServiceResult<RoomResponseDto>.BadRequest("Floor must be greater than zero.");
            }

            var existingRoom = await _dbContext.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                return ServiceResult<RoomResponseDto>.NotFound("Room not found.");
            }

            if (await _dbContext.Rooms.AnyAsync(r => r.Number == dto.Number.Trim() && r.Id != id))
            {
                return ServiceResult<RoomResponseDto>.Conflict("Room number already exists.");
            }

            var roomType = await _dbContext.RoomTypes.FindAsync(dto.RoomTypeId);
            if (roomType == null)
            {
                return ServiceResult<RoomResponseDto>.BadRequest("Room type does not exist.");
            }

            existingRoom.Number = dto.Number.Trim();
            existingRoom.Floor = dto.Floor;
            existingRoom.IsAvailable = dto.IsAvailable;
            existingRoom.RoomTypeId = dto.RoomTypeId;

            await _dbContext.SaveChangesAsync();

            return ServiceResult<RoomResponseDto>.Success(new RoomResponseDto
            {
                Id = existingRoom.Id,
                Number = existingRoom.Number,
                Floor = existingRoom.Floor,
                IsAvailable = existingRoom.IsAvailable,
                RoomTypeId = existingRoom.RoomTypeId,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.PricePerNight
            });
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var room = await _dbContext.Rooms.Include(r => r.Bookings).FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
            {
                return ServiceResult<bool>.NotFound("Room not found.");
            }

            if (room.Bookings.Any(b => b.CheckOut > DateTime.UtcNow))
            {
                return ServiceResult<bool>.Conflict("Cannot delete a room with active bookings.");
            }

            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
            return ServiceResult<bool>.Success(true);
        }
    }
}
