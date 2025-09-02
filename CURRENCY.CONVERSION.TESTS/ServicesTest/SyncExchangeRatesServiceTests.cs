using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using CURRENCY.CONVERSION.API.DTOs;
using CURRENCY.CONVERSION.API.Interfaces;
using CURRENCY.CONVERSION.API.Models;
using CURRENCY.CONVERSION.API.Services;
using System.Xml.Serialization;
using System.IO;

namespace CURRENCY.CONVERSION.TESTS.ServicesTest
{
    [TestFixture]
    public class SyncExchangeRatesServiceTests
    {
        private Mock<ICurrencyRateRepository> _repoMock;
        private Mock<HttpMessageHandler> _httpHandlerMock;
        private HttpClient _httpClient;
        private SyncExchangeRatesService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ICurrencyRateRepository>();
            _httpHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpHandlerMock.Object);
            _service = new SyncExchangeRatesService(_httpClient, _repoMock.Object);
        }

        [Test]
        public async Task SyncExchangeRatesAsync_ValidXml_UpsertsRates()
        {
            // Arrange
            var exchangeRateDto = new ExchangeRateDto
            {
                Type = "Exchange rates",
                ReferenceCurrency = "DKK",
                Author = "Nationalbanken",
                DailyRates = new DailyRates
                {
                    Currencies = new List<Currency>
                    {
                        new() { Code = "USD", Description = "US Dollar", Rate = 7.5m },
                        new() { Code = "EUR", Description = "Euro", Rate = 7.45m }
                    }
                }
            };

            var serializer = new XmlSerializer(typeof(ExchangeRateDto));
            string xml;
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, exchangeRateDto);
                xml = sw.ToString();
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(xml)
            };

            _httpHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            List<CurrencyRate>? upsertedRates = null;
            _repoMock
                .Setup(r => r.UpsertManyAsync(It.IsAny<IEnumerable<CurrencyRate>>()))
                .Callback<IEnumerable<CurrencyRate>>(rates => upsertedRates = [.. rates])
                .Returns(Task.CompletedTask);

            // Act
            await _service.SyncExchangeRatesAsync();

            // Assert
            Assert.That(upsertedRates, Is.Not.Null);
            Assert.That(upsertedRates!.Count, Is.EqualTo(3)); // 2 currencies + reference
            Assert.That(upsertedRates.Exists(r => r.Code == "DKK" && r.Rate == 100));
            Assert.That(upsertedRates.Exists(r => r.Code == "USD" && r.Rate == 7.5m));
            Assert.That(upsertedRates.Exists(r => r.Code == "EUR" && r.Rate == 7.45m));
        }
    }
}
