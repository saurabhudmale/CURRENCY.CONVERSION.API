using System;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.Interfaces;
using Quartz;

namespace CURRENCY.CONVERSION.API.Jobs
{
    public class SyncExchangeRatesJob : IJob
    {
        private readonly ISyncExchangeRatesService _syncExchangeRatesService;

        public SyncExchangeRatesJob(ISyncExchangeRatesService syncExchangeRatesService)
        {
            _syncExchangeRatesService = syncExchangeRatesService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _syncExchangeRatesService.SyncExchangeRatesAsync();
                Console.WriteLine("Exchange Rates Syncing Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error syncing exchange rates: " + ex.Message);
            }
        }
    }
}