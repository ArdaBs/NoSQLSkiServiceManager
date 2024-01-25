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
        private const int MaxLoginAttempts = 3;

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

        public async Task<string> AuthenticateEmployeeAsync(EmployeeLoginDto loginDto)
        {
            var employee = await _employees.Find(emp => emp.Username == loginDto.Username).FirstOrDefaultAsync();
            if (employee == null)
            {
                return "Benutzername nicht gefunden.";
            }

            if (employee.IsLocked)
            {
                return "Benutzerkonto ist gesperrt.";
            }

            if (employee.Password != loginDto.Password)
            {
                employee.FailedLoginAttempts += 1;
                await UpdateFailedLoginAttempts(employee);

                if (employee.FailedLoginAttempts >= 3)
                {
                    employee.IsLocked = true;
                    await LockEmployeeAccount(employee);
                    return "Benutzerkonto wurde wegen zu vieler fehlgeschlagener Versuche gesperrt.";
                }

                int remainingAttempts = 3 - employee.FailedLoginAttempts;
                return $"Falsches Passwort. Verbleibende Versuche: {remainingAttempts}";
            }

            employee.FailedLoginAttempts = 0;
            await ResetFailedLoginAttempts(employee);
            return null;
        }

        private async Task UpdateFailedLoginAttempts(Employee employee)
        {
            var update = Builders<Employee>.Update.Set(emp => emp.FailedLoginAttempts, employee.FailedLoginAttempts);
            await _employees.UpdateOneAsync(emp => emp.Id == employee.Id, update);
        }

        private async Task LockEmployeeAccount(Employee employee)
        {
            var update = Builders<Employee>.Update.Set(emp => emp.IsLocked, true);
            await _employees.UpdateOneAsync(emp => emp.Id == employee.Id, update);
        }

        private async Task ResetFailedLoginAttempts(Employee employee)
        {
            var update = Builders<Employee>.Update.Set(emp => emp.FailedLoginAttempts, 0);
            await _employees.UpdateOneAsync(emp => emp.Id == employee.Id, update);
        }

        public async Task<bool> UnlockEmployeeAccount(string username)
        {
            var employee = await _employees.Find(emp => emp.Username == username).FirstOrDefaultAsync();
            if (employee == null)
            {
                return false;
            }

            if (!employee.IsLocked)
            {
                return false;
            }

            // Entsperrung des Benutzerkontos
            var update = Builders<Employee>.Update.Set(emp => emp.IsLocked, false)
                                                  .Set(emp => emp.FailedLoginAttempts, 0);
            var result = await _employees.UpdateOneAsync(emp => emp.Id == employee.Id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }


        public async Task<Employee> GetEmployeeByUsernameAsync(string username)
        {
            return await _employees.Find(emp => emp.Username == username).FirstOrDefaultAsync();
        }
    }
}