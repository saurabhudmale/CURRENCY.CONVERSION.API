using System.Collections.Generic;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.DTOs;

namespace CURRENCY.CONVERSION.API.Interfaces
{
    public interface ICurrencyService
    {
        /// <summary>
        /// Returns all currency rates.
        /// </summary>
        Task<IEnumerable<CurrencyRateResponseDto>> GetAllExchangeRatesAsync();

        /// <summary>
        /// Gets a single currency rate by currency code.
        /// </summary>
        Task<CurrencyRateResponseDto?> GetExchangeRateAsync(string currencyCode);

        /// <summary>
        /// Converts the given amount.
        /// </summary>
        Task<ConversionResponseDto> ConvertAsync(ConversionRequestDto conversionRequestDto);
    }
}
