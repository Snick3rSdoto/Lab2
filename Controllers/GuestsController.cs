using HotelBookingApi.DTOs;
using HotelBookingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly GuestService _service;

        public GuestsController(GuestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestResponseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuestResponseDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<GuestResponseDto>> Create(GuestCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(dto);
            if (result.IsConflict) return Conflict(result.ErrorMessage);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GuestResponseDto>> Update(int id, GuestUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsNotFound) return NotFound(result.ErrorMessage);
            if (result.IsConflict) return Conflict(result.ErrorMessage);
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
