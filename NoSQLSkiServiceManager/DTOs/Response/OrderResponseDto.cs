using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.DTOs.Response
{
    public class OrderResponseDto
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DesiredPickupDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
    }
}
