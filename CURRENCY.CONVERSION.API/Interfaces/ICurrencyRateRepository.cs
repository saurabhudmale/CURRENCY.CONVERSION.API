using System.Collections.Generic;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.Models;

namespace CURRENCY.CONVERSION.API.Interfaces
{
    public interface ICurrencyRateRepository
    {
        Task UpsertManyAsync(IEnumerable<CurrencyRate> rates);
        Task<IEnumerable<CurrencyRate>> GetAllAsync();
        Task<CurrencyRate?> GetByCurrencyAsync(string currencyCode);
    }
}