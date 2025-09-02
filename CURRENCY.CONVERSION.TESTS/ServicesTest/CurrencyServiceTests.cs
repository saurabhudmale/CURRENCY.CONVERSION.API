using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Services;
using CURRENCY.CONVERSION.API.Models;

namespace CURRENCY.CONVERSION.TESTS
{
    [TestFixture]
    public class CurrencyServiceTests
    {
        private Mock<ICurrencyRateRepository> _repoMock;
        private CurrencyService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ICurrencyRateRepository>();
            _service = new CurrencyService(_repoMock.Object);
        }

        [Test]
        public async Task GetAllExchangeRatesAsync_ReturnsMappedRates()
        {
            var rates = new List<CurrencyRate>
            {
                new CurrencyRate { Code = "USD", Description = "US Dollar", Rate = 7.0m },
                new CurrencyRate { Code = "EUR", Description = "Euro", Rate = 7.5m }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(rates);

            var result = await _service.GetAllExchangeRatesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(r => r.CurrencyCode == "USD" && r.CurrencyDescription == "US Dollar" && r.Rate == 7.0m));
            Assert.That(result.Any(r => r.CurrencyCode == "EUR" && r.CurrencyDescription == "Euro" && r.Rate == 7.5m));
        }

        [Test]
        public async Task GetExchangeRateAsync_ValidCode_ReturnsRate()
        {
            var rate = new CurrencyRate { Code = "USD", Description = "US Dollar", Rate = 7.0m };
            _repoMock.Setup(r => r.GetByCurrencyAsync("USD")).ReturnsAsync(rate);

            var result = await _service.GetExchangeRateAsync("usd");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
            Assert.That(result.CurrencyDescription, Is.EqualTo("US Dollar"));
            Assert.That(result.Rate, Is.EqualTo(7.0m));
        }

        [Test]
        public async Task GetExchangeRateAsync_UnknownCode_ReturnsNull()
        {
            _repoMock.Setup(r => r.GetByCurrencyAsync("ABC")).ReturnsAsync((CurrencyRate?)null);

            var result = await _service.GetExchangeRateAsync("ABC");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetExchangeRateAsync_NullOrEmptyCode_ThrowsArgumentException()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _service.GetExchangeRateAsync(null));
            Assert.ThrowsAsync<ArgumentException>(() => _service.GetExchangeRateAsync(""));
            Assert.ThrowsAsync<ArgumentException>(() => _service.GetExchangeRateAsync("   "));
        }

        [Test]
        public async Task ConvertAsync_ValidRequest_ReturnsConversion()
        {
            var fromRate = new CurrencyRate { Code = "USD", Description = "US Dollar", Rate = 7.0m };
            var toRate = new CurrencyRate { Code = "DKK", Description = "Danish Krone", Rate = 1.0m };
            var request = new ConversionRequestDto("usd", "dkk", 10m);

            _repoMock.Setup(r => r.GetByCurrencyAsync("USD")).ReturnsAsync(fromRate);
            _repoMock.Setup(r => r.GetByCurrencyAsync("DKK")).ReturnsAsync(toRate);

            var result = await _service.ConvertAsync(request);

            Assert.That(result.FromCurrency, Is.EqualTo("USD"));
            Assert.That(result.ToCurrency, Is.EqualTo("DKK"));
            Assert.That(result.OriginalAmount, Is.EqualTo(10m));
            Assert.That(result.ConvertedAmount, Is.EqualTo(70m));
            Assert.That(result.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void ConvertAsync_NullOrEmptyCurrency_ThrowsArgumentException()
        {
            var request = new ConversionRequestDto(null, "DKK", 10m);
            Assert.ThrowsAsync<ArgumentException>(() => _service.ConvertAsync(request));

            request = new ConversionRequestDto("usd", null, 10m);
            Assert.ThrowsAsync<ArgumentException>(() => _service.ConvertAsync(request));

            request = new ConversionRequestDto("", "DKK", 10m);
            Assert.ThrowsAsync<ArgumentException>(() => _service.ConvertAsync(request));
        }

        [Test]
        public void ConvertAsync_AmountLessThanOrEqualZero_ThrowsArgumentException()
        {
            var request = new ConversionRequestDto("USD", "DKK", 0m);
            Assert.ThrowsAsync<ArgumentException>(() => _service.ConvertAsync(request));

            request = new ConversionRequestDto("USD", "DKK", -5m);
            Assert.ThrowsAsync<ArgumentException>(() => _service.ConvertAsync(request));
        }

        [Test]
        public void ConvertAsync_FromCurrencyNotFound_ThrowsKeyNotFoundException()
        {
            var request = new ConversionRequestDto("USD", "DKK", 10m);
            _repoMock.Setup(r => r.GetByCurrencyAsync("USD")).ReturnsAsync((CurrencyRate?)null);
            _repoMock.Setup(r => r.GetByCurrencyAsync("DKK")).ReturnsAsync(new CurrencyRate { Code = "DKK", Rate = 1.0m });

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ConvertAsync(request));
        }

        [Test]
        public void ConvertAsync_ToCurrencyNotFound_ThrowsKeyNotFoundException()
        {
            var request = new ConversionRequestDto("USD", "DKK", 10m);
            _repoMock.Setup(r => r.GetByCurrencyAsync("USD")).ReturnsAsync(new CurrencyRate { Code = "USD", Rate = 7.0m });
            _repoMock.Setup(r => r.GetByCurrencyAsync("DKK")).ReturnsAsync((CurrencyRate?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ConvertAsync(request));
        }
    }
}
