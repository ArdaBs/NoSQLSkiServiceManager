namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class EmployeeResponseDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
    }

}
