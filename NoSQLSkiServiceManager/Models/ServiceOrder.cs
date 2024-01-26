using MongoDB.Bson;
using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.Models
{
    public class ServiceOrder : IEntity
    {
        public ObjectId Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime PickupDate { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public ServiceType ServiceType { get; set; }
        public ServiceOrderPriority Priority { get; set; }
    }

}
