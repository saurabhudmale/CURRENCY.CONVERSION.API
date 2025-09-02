using System;

namespace CURRENCY.CONVERSION.API.DTOs
{
    /// <summary>
    /// Represents a stored record of a conversion.
    /// </summary>
    public record ConversionHistoryResponseDto
    (
        Guid Id,
        string FromCurrency,
        string ToCurrency,
        decimal Amount,
        decimal ConvertedAmount,
        DateTime Timestamp
    );
}