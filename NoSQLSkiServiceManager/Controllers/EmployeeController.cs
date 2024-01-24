using Microsoft.AspNetCore.Mvc;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.Services;

namespace NoSQLSkiServiceManager.Controllers
{
    public class EmployeeController : GenericController<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto>
    {
        public EmployeeController(GenericService<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto> service) : base(service)
        {
        }

    }

}
