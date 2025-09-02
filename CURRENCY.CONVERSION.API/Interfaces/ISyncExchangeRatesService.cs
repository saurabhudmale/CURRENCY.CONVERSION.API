using System.Threading.Tasks;

namespace CURRENCY.CONVERSION.API.Interfaces
{
    public interface ISyncExchangeRatesService
    {
        Task SyncExchangeRatesAsync();
    }
}
