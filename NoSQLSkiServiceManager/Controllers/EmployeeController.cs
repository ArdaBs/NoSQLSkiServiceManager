using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace NoSQLSkiServiceManager.Controllers
{
    public class EmployeeController : GenericController<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto>
    {
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly EmployeeService _employeeService;

        public EmployeeController(GenericService<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto> genericService,
                                  EmployeeService employeeService,
                                  TokenService tokenService,
                                  IMapper mapper) : base(genericService)
        {
            _employeeService = employeeService;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto loginDto)
        {
            var result = await _employeeService.AuthenticateEmployeeAsync(loginDto);

            if (result != null)
            {
                return Unauthorized(result);
            }

            var token = _tokenService.CreateToken(loginDto.Username);
            return Ok(new { Token = token });
        }

        [HttpPost("unlock/{username}")]
        [Authorize]
        public async Task<IActionResult> UnlockAccount(string username)
        {
            var success = await _employeeService.UnlockEmployeeAccount(username);

            if (!success)
            {
                return NotFound("Benutzerkonto nicht gefunden oder nicht gesperrt.");
            }

            return Ok("Benutzerkonto erfolgreich entsperrt.");
        }

    }

}