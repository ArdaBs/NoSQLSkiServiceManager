using MongoDB.Bson;
using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NoSQLSkiServiceManager.Services
{
    /// <summary>
    /// Provides service layer functionality for employee operations.
    /// </summary>
    public class EmployeeService : GenericService<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto>
    {
        private const int MaxLoginAttempts = 3;

        /// <summary>
        /// Initializes a new instance of the EmployeeService class.
        /// </summary>
        /// <param name="database">The Mongo database connection.</param>
        /// <param name="mapper">The class used for object mapping.</param>
        public EmployeeService(IMongoDatabase database, IMapper mapper)
            : base(database, mapper, "employees")
        {
        }

        /// <summary>
        /// Authenticates an employee's login attempt.
        /// </summary>
        /// <param name="loginDto">The data transfer object containing login credentials.</param>
        /// <returns>A message indicating the result of the authentication attempt.</returns>
        public async Task<string> AuthenticateEmployeeAsync(EmployeeLoginDto loginDto)
        {
            var employee = await _collection.Find(emp => emp.Username == loginDto.Username).FirstOrDefaultAsync();
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
                if (employee.FailedLoginAttempts >= MaxLoginAttempts)
                {
                    employee.IsLocked = true;
                    var lockUpdate = Builders<Employee>.Update.Set(emp => emp.IsLocked, true);
                    await _collection.UpdateOneAsync(emp => emp.Id == employee.Id, lockUpdate);
                    return "Benutzerkonto wurde wegen zu vieler fehlgeschlagener Versuche gesperrt.";
                }
                else
                {
                    var attemptsUpdate = Builders<Employee>.Update.Set(emp => emp.FailedLoginAttempts, employee.FailedLoginAttempts);
                    await _collection.UpdateOneAsync(emp => emp.Id == employee.Id, attemptsUpdate);
                    int remainingAttempts = MaxLoginAttempts - employee.FailedLoginAttempts;
                    return $"Falsches Passwort. Verbleibende Versuche: {remainingAttempts}";
                }
            }

            var resetAttemptsUpdate = Builders<Employee>.Update.Set(emp => emp.FailedLoginAttempts, 0);
            await _collection.UpdateOneAsync(emp => emp.Id == employee.Id, resetAttemptsUpdate);
            return null;
        }

        /// <summary>
        /// Unlocks an employee's account.
        /// </summary>
        /// <param name="username">The username of the employee whose account is to be unlocked.</param>
        /// <returns>A boolean indicating whether the account was successfully unlocked.</returns>
        public async Task<bool> UnlockEmployeeAccount(string username)
        {
            var filter = Builders<Employee>.Filter.Eq(emp => emp.Username, username) & Builders<Employee>.Filter.Eq(emp => emp.IsLocked, true);
            var update = Builders<Employee>.Update.Set(emp => emp.IsLocked, false).Set(emp => emp.FailedLoginAttempts, 0);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
