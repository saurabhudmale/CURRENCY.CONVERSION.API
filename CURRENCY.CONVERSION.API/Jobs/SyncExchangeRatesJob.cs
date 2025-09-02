using System;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.Interfaces;
using Quartz;
using Serilog;

namespace CURRENCY.CONVERSION.API.Jobs
{
    public class SyncExchangeRatesJob : IJob
    {
        private readonly ILogger _serilog;
        private readonly ISyncExchangeRatesService _syncExchangeRatesService;

        public SyncExchangeRatesJob(ILogger serilog, ISyncExchangeRatesService syncExchangeRatesService)
        {
            _serilog = serilog;
            _syncExchangeRatesService = syncExchangeRatesService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _syncExchangeRatesService.SyncExchangeRatesAsync();

                _serilog.Information("Exchange rates syncing completed successfully");
            }
            catch (Exception ex)
            {
                _serilog.Error(ex, ex.Message, "Error in syncing exchange rates");
            }
        }
    }
}