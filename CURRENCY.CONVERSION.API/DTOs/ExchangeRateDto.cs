using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CURRENCY.CONVERSION.API.DTOs
{
    [XmlRoot("exchangerates")]
    public class ExchangeRateDto
    {
        [XmlAttribute("type")]
        public required string Type { get; set; }

        [XmlAttribute("author")]
        public required string Author { get; set; }

        [XmlAttribute("refcur")]
        public required string ReferenceCurrency { get; set; }

        [XmlAttribute("refamt")]
        public decimal ReferenceAmount { get; set; }

        [XmlElement("dailyrates")]
        public DailyRates? DailyRates { get; set; }
    }

    public class DailyRates
    {
        [XmlAttribute("id")]
        public DateTime Id { get; set; }

        [XmlElement("currency")]
        public List<Currency> Currencies { get; set; } = new();
    }

    public class Currency
    {
        [XmlAttribute("code")]
        public required string Code { get; set; }

        [XmlAttribute("desc")]
        public required string Description { get; set; }

        [XmlAttribute("rate")]
        public decimal Rate { get; set; }
    }
}