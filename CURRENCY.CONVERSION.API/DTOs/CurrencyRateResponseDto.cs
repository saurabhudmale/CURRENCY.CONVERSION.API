using System;

namespace CURRENCY.CONVERSION.API.DTOs
{
    /// <summary>
    /// Represents a currency rate with respect to DKK.
    /// </summary>
    public record CurrencyRateResponseDto(string CurrencyCode, string CurrencyDescription, decimal Rate);
}