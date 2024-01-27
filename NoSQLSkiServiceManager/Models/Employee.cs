using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NoSQLSkiServiceManager.Interfaces;

namespace NoSQLSkiServiceManager.Models
{
    public class Employee : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("isLocked")]
        public bool IsLocked { get; set; }

        [BsonElement("failedLoginAttempts")]
        public int FailedLoginAttempts { get; set; }
    }
}
