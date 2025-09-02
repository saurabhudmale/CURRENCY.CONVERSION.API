using CURRENCY.CONVERSION.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CURRENCY.CONVERSION.API.Interfaces
{
    public interface IConversionHistoryService
    {
        /// <summary>
        /// Saves a record of a conversion to the database.
        /// </summary>
        Task SaveConversionAsync(string fromCurrency, string toCurrency, decimal amount, decimal convertedAmount);

        /// <summary>
        /// Returns all conversion history with optional filters.
        /// </summary>
        Task<IEnumerable<ConversionHistoryResponseDto>> GetConversionHistoryAsync(string? fromCurrency = null, DateTime? from = null, DateTime? to = null);
    }
}