using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Controllers
{
    public class EmployeeController : GenericController<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto>
    {
        private readonly TokenService _tokenService;

        public EmployeeController(GenericService<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto> service, TokenService tokenService)
            : base(service)
        {
            _tokenService = tokenService;
        }
        
        /*
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto loginDto)
        {
            var employee = await Service.GetByIdAsync(loginDto.Username);
            if (employee == null || employee.Password != loginDto.Password)
            {
                return Unauthorized("Ungültige Anmeldedaten.");
            }

            var token = _tokenService.CreateToken(employee.Username);

            return Ok(new { Message = "Erfolgreich eingeloggt", Token = token });
        }
        */
    }
}
