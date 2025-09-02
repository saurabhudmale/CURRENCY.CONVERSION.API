using System;
using System.Threading.Tasks;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CURRENCY.CONVERSION.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IConversionHistoryService _conversionHistoryService;

        public CurrencyController(ICurrencyService currencyService, IConversionHistoryService conversionHistoryService)
        {
            _currencyService = currencyService;
            _conversionHistoryService = conversionHistoryService;
        }

        /// <summary>
        /// Gets all available currency rates.
        /// </summary>
        [HttpGet("GetExchangeRates")]
        public async Task<IActionResult> GetExchangeRatesAsync()
        {
            try
            {
                return Ok(await _currencyService.GetAllExchangeRatesAsync());
            }
            catch (Exception ex)
            {
                return ReturnError(ex);
            }
        }

        /// <summary>
        /// Gets a single currency rate by currency code.
        /// </summary>
        [HttpGet("GetRate/{currencyCode}")]
        public async Task<IActionResult> GetRateAsync(string currencyCode)
        {
            try
            {
                return Ok(await _currencyService.GetExchangeRateAsync(currencyCode));
            }
            catch (Exception ex)
            {
                return ReturnError(ex);
            }
        }

        /// <summary>
        /// Converts an amount from a given currency into DKK.
        /// </summary>
        [HttpPost("Convert")]
        public async Task<IActionResult> ConvertAsync([FromBody] ConversionRequestDto conversionRequestDto)
        {
            try
            {
                var response = await _currencyService.ConvertAsync(conversionRequestDto);

                await _conversionHistoryService.SaveConversionAsync
                (
                    conversionRequestDto.FromCurrency,
                    conversionRequestDto.ToCurrency,
                    conversionRequestDto.Amount,
                    response.ConvertedAmount
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return ReturnError(ex);
            }
        }

        /// <summary>
        /// Fetches conversion history with optional filters.
        /// </summary>
        [HttpGet("GetConversionHistory")]
        public async Task<IActionResult> GetConversionHistoryAsync
        (
            [FromQuery] string? fromCurrency = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null
        )
        {
            try
            {
                return Ok(await _conversionHistoryService.GetConversionHistoryAsync(fromCurrency, from, to));
            }
            catch (Exception ex)
            {
                return ReturnError(ex);
            }
        }

        [NonAction]
        private IActionResult ReturnError(Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}