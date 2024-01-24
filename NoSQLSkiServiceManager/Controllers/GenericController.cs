using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Interfaces;
using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<TModel, TCreateDto, TUpdateDto, TResponseDto> : ControllerBase
        where TModel : class, IEntity
        where TCreateDto : class
        where TUpdateDto : class
        where TResponseDto : class, IResponseDto
    {
        private readonly GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> _service;

        public GenericController(GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TCreateDto createDto)
        {
            var createdItem = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, TUpdateDto updateDto)
        {
            var updated = await _service.UpdateAsync(id, updateDto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
