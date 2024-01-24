namespace NoSQLSkiServiceManager.DTOs.Requests
{
    public class CreateServiceOrderRequestDto
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime DesiredPickupDate { get; set; }
        public string Comments { get; set; }
    }
}
