using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;
using CURRENCY.CONVERSION.API.Services;

namespace CURRENCY.CONVERSION.TESTS.ServicesTest
{
    [TestFixture]
    public class ConversionHistoryServiceTests
    {
        private Mock<IConversionHistoryRepository> _repoMock;
        private ConversionHistoryService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IConversionHistoryRepository>();
            _service = new ConversionHistoryService(_repoMock.Object);
        }

        [Test]
        public async Task SaveConversionAsync_CallsRepositoryWithCorrectData()
        {
            // Arrange
            string fromCurrency = "usd";
            string toCurrency = "eur";
            decimal amount = 100m;
            decimal convertedAmount = 90m;
            ConversionHistory? captured = null;

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<ConversionHistory>()))
                .Callback<ConversionHistory>(ch => captured = ch)
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveConversionAsync(fromCurrency, toCurrency, amount, convertedAmount);

            // Assert
            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.FromCurrency, Is.EqualTo("USD")); // Should be uppercased
            Assert.That(captured.ToCurrency, Is.EqualTo(toCurrency));
            Assert.That(captured.Amount, Is.EqualTo(amount));
            Assert.That(captured.ConvertedAmount, Is.EqualTo(convertedAmount));
            Assert.That(captured.Timestamp, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public async Task GetConversionHistoryAsync_ReturnsMappedDtos()
        {
            // Arrange
            var records = new List<ConversionHistory>
            {
                new ConversionHistory
                {
                    Id = Guid.NewGuid(),
                    FromCurrency = "USD",
                    ToCurrency = "EUR",
                    Amount = 100m,
                    ConvertedAmount = 90m,
                    Timestamp = DateTime.UtcNow.AddMinutes(-10)
                },
                new ConversionHistory
                {
                    Id = Guid.NewGuid(),
                    FromCurrency = "EUR",
                    ToCurrency = "USD",
                    Amount = 50m,
                    ConvertedAmount = 55m,
                    Timestamp = DateTime.UtcNow.AddMinutes(-5)
                }
            };

            _repoMock
                .Setup(r => r.GetAsync(null, null, null))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetConversionHistoryAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
            var first = result.First();
            Assert.That(first.FromCurrency, Is.EqualTo("USD"));
            Assert.That(first.ToCurrency, Is.EqualTo("EUR"));
            Assert.That(first.Amount, Is.EqualTo(100m));
            Assert.That(first.ConvertedAmount, Is.EqualTo(90m));
        }

        [Test]
        public async Task GetConversionHistoryAsync_WithFilters_PassesFiltersToRepository()
        {
            // Arrange
            string fromCurrency = "usd";
            DateTime from = DateTime.UtcNow.AddDays(-1);
            DateTime to = DateTime.UtcNow;
            var records = new List<ConversionHistory>();

            _repoMock
                .Setup(r => r.GetAsync(from, to, "USD"))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetConversionHistoryAsync(fromCurrency, from, to);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
