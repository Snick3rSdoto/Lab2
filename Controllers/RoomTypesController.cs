using HotelBookingApi.DTOs;
using HotelBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypesController : ControllerBase
    {
        private readonly RoomTypeService _service;

        public RoomTypesController(RoomTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeResponseDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomTypeResponseDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeResponseDto>> Create(RoomTypeCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoomTypeResponseDto>> Update(int id, RoomTypeUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            if (result.IsConflict) return Conflict(result.ErrorMessage);
            return NoContent();
        }
    }
}
