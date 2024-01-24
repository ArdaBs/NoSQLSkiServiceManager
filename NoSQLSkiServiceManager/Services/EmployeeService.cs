using MongoDB.Bson;
using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;

namespace NoSQLSkiServiceManager.Services
{

    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;
        private readonly IMapper _mapper;

        public EmployeeService(IMongoDatabase database, IMapper mapper)
        {
            _employees = database.GetCollection<Employee>("Employees");
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
        {
            var employees = await _employees.Find(emp => true).ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeResponseDto>>(employees);
        }

        public async Task<EmployeeResponseDto> GetByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var employee = await _employees.Find(emp => emp.Id == objectId).FirstOrDefaultAsync();
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto createDto)
        {
            var employee = _mapper.Map<Employee>(createDto);
            await _employees.InsertOneAsync(employee);
            return _mapper.Map<EmployeeResponseDto>(employee);
        }

        public async Task<bool> UpdateAsync(string id, EmployeeUpdateDto updateDto)
        {
            var objectId = ObjectId.Parse(id);
            var employee = _mapper.Map<Employee>(updateDto);
            employee.Id = objectId;
            var result = await _employees.ReplaceOneAsync(emp => emp.Id == objectId, employee);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var result = await _employees.DeleteOneAsync(emp => emp.Id == objectId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}