using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;

namespace CURRENCY.CONVERSION.API.Services
{
    public class ConversionHistoryService : IConversionHistoryService
    {
        private readonly IConversionHistoryRepository _conversionHistoryRepository;

        public ConversionHistoryService(IConversionHistoryRepository conversionHistoryRepository)
        {
            _conversionHistoryRepository = conversionHistoryRepository;
        }

        /// <summary>
        /// Saves a record of the conversion.
        /// </summary>
        public async Task SaveConversionAsync(string fromCurrency, string toCurrency, decimal amount, decimal convertedAmount)
        {
            var record = new ConversionHistory
            {
                FromCurrency = fromCurrency.ToUpperInvariant(),
                ToCurrency = toCurrency,
                Amount = amount,
                ConvertedAmount = convertedAmount,
                Timestamp = DateTime.UtcNow
            };

            await _conversionHistoryRepository.AddAsync(record);
        }

        /// <summary>
        /// Fetches conversion history with optional filters.
        /// </summary>
        public async Task<IEnumerable<ConversionHistoryResponseDto>> GetConversionHistoryAsync
        (
            string? fromCurrency = null,
            DateTime? from = null,
            DateTime? to = null
        )
        {
            var records = await _conversionHistoryRepository.GetAsync(from, to, fromCurrency?.ToUpperInvariant());

            return records.Select(r => new ConversionHistoryResponseDto
            (
                r.Id,
                r.FromCurrency,
                r.ToCurrency,
                r.Amount,
                r.ConvertedAmount,
                r.Timestamp
            ));
        }
    }
}