﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NoSQLSkiServiceManager.Models
{
    public class ServiceType
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("cost")]
        public decimal Cost { get; set; }
    }
}
