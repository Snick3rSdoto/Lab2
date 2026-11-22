using HotelBookingApi.DTOs;
using HotelBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingsController(BookingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingResponseDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> Create(BookingCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            if (result.IsConflict) return Conflict(result.ErrorMessage);
            if (result.IsBadRequest) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BookingResponseDto>> Update(int id, BookingUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            if (result.IsConflict) return Conflict(result.ErrorMessage);
            if (result.IsBadRequest) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            return NoContent();
        }
    }
}
