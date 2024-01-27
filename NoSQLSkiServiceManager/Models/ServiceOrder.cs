using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NoSQLSkiServiceManager.Interfaces;
using System;

namespace NoSQLSkiServiceManager.Models
{
    public class ServiceOrder : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("customerName")]
        public string CustomerName { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("pickupDate")]
        public DateTime PickupDate { get; set; }

        [BsonElement("desiredPickupDate")]
        public DateTime DesiredPickupDate { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("serviceType")]
        public ServiceType ServiceType { get; set; }

        [BsonElement("priority")]
        public ServicePriority Priority { get; set; }
    }
}
