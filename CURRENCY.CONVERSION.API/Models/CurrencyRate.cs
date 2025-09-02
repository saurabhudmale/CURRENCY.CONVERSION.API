using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CURRENCY.CONVERSION.API.Models
{
    public class CurrencyRate
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}