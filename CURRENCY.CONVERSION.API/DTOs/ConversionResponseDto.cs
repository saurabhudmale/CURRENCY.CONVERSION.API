using System;

namespace CURRENCY.CONVERSION.API.DTOs
{
    /// <summary>
    /// Response DTO after performing a currency conversion.
    /// </summary>
    public record ConversionResponseDto
    (
        string FromCurrency,
        string ToCurrency,
        decimal OriginalAmount,
        decimal ConvertedAmount,
        DateTime Timestamp
    );
}