using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Controllers
{
    public class ServiceOrderController : GenericController<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto>
    {
        private readonly ServiceOrderService _serviceOrderService;

        public ServiceOrderController(
            GenericService<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto> genericService,
            ServiceOrderService serviceOrderService) : base(genericService)
        {
            _serviceOrderService = serviceOrderService;
        }

        [HttpPost]
        public override async Task<IActionResult> Create(CreateServiceOrderRequestDto createDto)
        {
            var createdOrder = await _serviceOrderService.CreateWithDetails(createDto);
            if (createdOrder == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(Get), new { id = createdOrder.Id }, createdOrder);
        }

    }


}
