using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using CURRENCY.CONVERSION.API.Models;

namespace CURRENCY.CONVERSION.API.Interfaces
{
    public interface IConversionHistoryRepository
    {
        Task AddAsync(ConversionHistory conversionHistory);
        Task<IEnumerable<ConversionHistory>> GetAsync(DateTime? from = null, DateTime? to = null, string? fromCurrency = null);
    }
}
