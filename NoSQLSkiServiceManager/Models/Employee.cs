using MongoDB.Bson;

namespace NoSQLSkiServiceManager.Models
{
    public class Employee
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
    }

}
