using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSkiServiceManager.Models
{
    public class ServicePriority
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonElement("priorityName")]
        public string PriorityName { get; set; }

        [BsonElement("dayCount")]
        public int DayCount { get; set; }
    }
}
