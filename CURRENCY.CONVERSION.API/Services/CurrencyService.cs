using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;

namespace CURRENCY.CONVERSION.API.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRateRepository _currencyRateRepository;

        public CurrencyService(ICurrencyRateRepository currencyRateRepository)
        {
            _currencyRateRepository = currencyRateRepository;
        }

        /// <summary>
        /// Gets all available currency rates.
        /// </summary>
        public async Task<IEnumerable<CurrencyRateResponseDto>> GetAllExchangeRatesAsync()
        {
            var rates = await _currencyRateRepository.GetAllAsync();
            return rates.Select(r => new CurrencyRateResponseDto(r.Code, r.Description, r.Rate));
        }

        /// <summary>
        /// Gets a specific currency rate by code.
        /// </summary>
        public async Task<CurrencyRateResponseDto?> GetExchangeRateAsync(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be null or empty.");

            var rate = await _currencyRateRepository.GetByCurrencyAsync(currencyCode.ToUpperInvariant());
            return rate == null ? null : new CurrencyRateResponseDto(rate.Code, rate.Description, rate.Rate);
        }

        /// <summary>
        /// Converts the given amount from the specified currency to DKK.
        /// </summary>
        public async Task<ConversionResponseDto> ConvertAsync(ConversionRequestDto conversionRequestDto)
        {
            if (string.IsNullOrWhiteSpace(conversionRequestDto.FromCurrency)
                || string.IsNullOrWhiteSpace(conversionRequestDto.ToCurrency))
                throw new ArgumentException("Currency code cannot be null or empty.");

            if (conversionRequestDto.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var fromCurrencyRateTask = _currencyRateRepository.GetByCurrencyAsync(conversionRequestDto.FromCurrency.ToUpperInvariant());
            var toCurrencyRateTask = _currencyRateRepository.GetByCurrencyAsync(conversionRequestDto.ToCurrency.ToUpperInvariant());
            var fromCurrencyRate = await fromCurrencyRateTask;
            var toCurrencyRate = await toCurrencyRateTask;

            if (fromCurrencyRate == null)
                throw new KeyNotFoundException($"Currency rate not found for {conversionRequestDto.FromCurrency}");

            if (toCurrencyRate == null)
                throw new KeyNotFoundException($"Currency rate not found for {conversionRequestDto.ToCurrency}");

            return new ConversionResponseDto
            (
                conversionRequestDto.FromCurrency.ToUpperInvariant(),
                conversionRequestDto.ToCurrency.ToUpperInvariant(),
                conversionRequestDto.Amount,
                (conversionRequestDto.Amount * fromCurrencyRate.Rate) / toCurrencyRate.Rate,
                DateTime.UtcNow
            );
        }
    }
}