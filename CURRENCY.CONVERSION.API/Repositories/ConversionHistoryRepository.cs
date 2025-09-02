using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CURRENCY.CONVERSION.API.Repositories
{
    public class ConversionHistoryRepository : IConversionHistoryRepository
    {
        private readonly IMongoCollection<ConversionHistory> _mongoCollection;

        public ConversionHistoryRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("MongoDbSettings:ConnectionString");
            var databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);

            _mongoCollection = db.GetCollection<ConversionHistory>("ConversionHistory");

            var indexKeys = Builders<ConversionHistory>.IndexKeys.Ascending(r => r.FromCurrency).Ascending(r => r.Timestamp);
            _mongoCollection.Indexes.CreateOne(new CreateIndexModel<ConversionHistory>(indexKeys));
        }

        public async Task AddAsync(ConversionHistory record)
        {
            await _mongoCollection.InsertOneAsync(record);
        }

        public async Task<IEnumerable<ConversionHistory>> GetAsync(DateTime? from = null, DateTime? to = null, string? fromCurrency = null)
        {
            var filter = Builders<ConversionHistory>.Filter.Empty;

            if (from.HasValue)
            {
                filter = Builders<ConversionHistory>.Filter.Gte(r => r.Timestamp, from.Value);
            }
            if (to.HasValue)
            {
                var toFilter = Builders<ConversionHistory>.Filter.Lte(r => r.Timestamp, to.Value);
                filter = filter == Builders<ConversionHistory>.Filter.Empty ? toFilter : Builders<ConversionHistory>.Filter.And(filter, toFilter);
            }
            if (!string.IsNullOrWhiteSpace(fromCurrency))
            {
                var currencyFilter = Builders<ConversionHistory>.Filter.Eq(r => r.FromCurrency, fromCurrency);
                filter = filter == Builders<ConversionHistory>.Filter.Empty ? currencyFilter : Builders<ConversionHistory>.Filter.And(filter, currencyFilter);
            }

            return await _mongoCollection.Find(filter).ToListAsync();
        }
    }
}