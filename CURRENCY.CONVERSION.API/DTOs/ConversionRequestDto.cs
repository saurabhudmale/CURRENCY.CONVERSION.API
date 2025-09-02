namespace CURRENCY.CONVERSION.API.DTOs
{
    /// <summary>
    /// DTO used to request currency conversion.
    /// </summary>
    public record ConversionRequestDto(string FromCurrency, string ToCurrency, decimal Amount);
}