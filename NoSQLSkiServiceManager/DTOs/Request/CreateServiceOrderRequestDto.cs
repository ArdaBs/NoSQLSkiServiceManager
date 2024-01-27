using MongoDB.Bson;
using NoSQLSkiServiceManager.Models;

namespace NoSQLSkiServiceManager.DTOs.Requests
{
    public class CreateServiceOrderRequestDto
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PickupDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string ServiceTypeId { get; set; }
        public string PriorityId { get; set; }
    }
}
