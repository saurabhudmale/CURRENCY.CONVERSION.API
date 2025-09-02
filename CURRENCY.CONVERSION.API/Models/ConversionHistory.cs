using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CURRENCY.CONVERSION.API.Models
{
    public class ConversionHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
