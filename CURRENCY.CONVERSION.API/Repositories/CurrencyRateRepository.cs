using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CURRENCY.CONVERSION.API.Repositories
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly IMongoCollection<CurrencyRate> _mongoCollection;
        public CurrencyRateRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("MongoDbSettings:ConnectionString");
            var databaseName = configuration.GetValue<string>("MongoDbSettings:DatabaseName");
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);

            _mongoCollection = db.GetCollection<CurrencyRate>("ExchangeRates");

            var indexKeys = Builders<CurrencyRate>.IndexKeys.Ascending(c => c.Code);
            _mongoCollection.Indexes.CreateOne(new CreateIndexModel<CurrencyRate>(indexKeys));
        }

        public async Task UpsertManyAsync(IEnumerable<CurrencyRate> rates)
        {
            if (rates == null || !rates.Any())
                return;

            var bulkOps = rates.Select(rate =>
            {
                var filter = Builders<CurrencyRate>.Filter.Eq(r => r.Code, rate.Code);
                var update = Builders<CurrencyRate>.Update
                    .SetOnInsert(r => r.Id, rate.Id)
                    .Set(r => r.Code, rate.Code)
                    .Set(r => r.Description, rate.Description)
                    .Set(r => r.Rate, rate.Rate)
                    .Set(r => r.Timestamp, rate.Timestamp);

                return new UpdateOneModel<CurrencyRate>(filter, update) { IsUpsert = true };
            });

            var result = await _mongoCollection.BulkWriteAsync(bulkOps, new BulkWriteOptions
            {
                IsOrdered = false // Faster because operations run in parallel internally
            });
        }

        public async Task<CurrencyRate?> GetByCurrencyAsync(string currencyCode)
        {
            return await _mongoCollection.Find(i => i.Code == currencyCode).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CurrencyRate>> GetAllAsync()
        {
            return await _mongoCollection.Find(_ => true).ToListAsync();
        }
    }
}
