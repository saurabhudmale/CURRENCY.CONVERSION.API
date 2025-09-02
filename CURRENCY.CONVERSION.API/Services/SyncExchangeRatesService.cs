using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;

namespace CURRENCY.CONVERSION.API.Services
{
    public class SyncExchangeRatesService : ISyncExchangeRatesService
    {
        private readonly HttpClient _httpClient;
        private readonly ICurrencyRateRepository _currencyRateRepository;

        public SyncExchangeRatesService(HttpClient httpClient, ICurrencyRateRepository currencyRateRepository)
        {
            _httpClient = httpClient;
            _currencyRateRepository = currencyRateRepository;
        }

        public async Task SyncExchangeRatesAsync()
        {
            var url = "https://www.nationalbanken.dk/api/currencyratesxml";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(ExchangeRateDto));

            var reader = new StringReader(xmlContent);

            var exchangeRateDto = serializer.Deserialize(reader) as ExchangeRateDto;

            if (exchangeRateDto != null && exchangeRateDto?.DailyRates != null)
            {
                var referenceCurrencyRate = new CurrencyRate
                {
                    Code = exchangeRateDto.ReferenceCurrency,
                    Description = exchangeRateDto.Author,
                    Rate = 100,
                    Timestamp = DateTime.UtcNow
                };

                var allCurrencyRate = exchangeRateDto.DailyRates.Currencies.Select(currency =>
                {
                    return new CurrencyRate
                    {
                        Code = currency.Code,
                        Description = currency.Description,
                        Rate = currency.Rate,
                        Timestamp = DateTime.UtcNow
                    };
                }).ToList();

                allCurrencyRate.Add(referenceCurrencyRate);

                await _currencyRateRepository.UpsertManyAsync(allCurrencyRate);
            }
        }
    }
}