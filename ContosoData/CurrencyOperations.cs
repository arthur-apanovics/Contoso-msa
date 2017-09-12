using System;
using ContosoData.Contollers;
using ContosoData.Model;
using Newtonsoft.Json;

namespace ContosoData
{
    public class CurrencyOperations
    {
        /// <summary>
        /// Get exchange rates based on a single currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns>ExchangeRate</returns>
        public ExchangeRate GetExchangeRate(string currency)
        {
            var abbreviation = GetCurrencyAbbreviation(currency);
            var rateJson = new ExchangeRateController().GetRates(abbreviation);
            ExchangeRate exchangeRate;

            try
            {
                exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(rateJson);
            }
            catch (JsonException e)
            {
                Console.WriteLine(e);
                return null;
            }

            return exchangeRate;
        }

        /// <summary>
        /// Get exchange rate for two specific currencies, ignoring all other currencies
        /// </summary>
        /// <param name="firstCurrency"></param>
        /// <param name="secondCurrency"></param>
        /// <returns>ExchangeRate</returns>
        public ExchangeRate GetExchangeRate(string firstCurrency, string secondCurrency)
        {
            var firstAbbr  = GetCurrencyAbbreviation(firstCurrency);
            var secondAbbr = GetCurrencyAbbreviation(secondCurrency);

            var rateJson = new ExchangeRateController().GetRates(firstAbbr, secondAbbr);
            ExchangeRate exchangeRate;

            try
            {
                exchangeRate = JsonConvert.DeserializeObject<ExchangeRate>(rateJson);
            }
            catch (JsonException e)
            {
                Console.WriteLine(e);
                return null;
            }

            return exchangeRate;
        }

        public float ConvertCurrency(string baseCurrency, string targetCurrency, float amount)
        {
            var targetAbbr       = GetCurrencyAbbreviation(targetCurrency);
            var exchangeRate     = GetExchangeRate(baseCurrency, targetCurrency);
            var targetRate       = (float) exchangeRate.Rates.GetType().GetProperty(targetAbbr).GetValue(exchangeRate.Rates);

            var conversionResult = amount / targetRate;

            return conversionResult;
        }

        private string GetCurrencyAbbreviation(string currency)
        {
            var abbreviation = string.Empty;

            switch (currency)
            {
                case "australian dollar":
                    abbreviation = "AUD";
                    break;
                case "bulgarian lev":
                    abbreviation = "BGN";
                    break;
                case "brazilian real":
                    abbreviation = "BRL";
                    break;
                case "canadian dollar":
                    abbreviation = "CAD";
                    break;
                case "swiss franc":
                    abbreviation = "CHF";
                    break;
                case "chinese yuan":
                    abbreviation = "CNY";
                    break;
                case "czech koruna":
                    abbreviation = "CZK";
                    break;
                case "danish krone":
                    abbreviation = "DKK";
                    break;
                case "british pound":
                    abbreviation = "GBP";
                    break;
                case "hong kong dollar":
                    abbreviation = "HKD";
                    break;
                case "croatian kuna":
                    abbreviation = "HRK";
                    break;
                case "hungarian forint":
                    abbreviation = "HUF";
                    break;
                case "indonesian rupiah":
                    abbreviation = "IDR";
                    break;
                case "israeli new shekel":
                    abbreviation = "ILS";
                    break;
                case "indian rupee":
                    abbreviation = "INR";
                    break;
                case "japanese yen":
                    abbreviation = "JPY";
                    break;
                case "south korean won":
                    abbreviation = "KRW";
                    break;
                case "mexican peso":
                    abbreviation = "MXN";
                    break;
                case "malaysian ringgit":
                    abbreviation = "MYR";
                    break;
                case "norwegian krone":
                    abbreviation = "NOK";
                    break;
                case "philippine peso":
                    abbreviation = "PHP";
                    break;
                case "polish złoty":
                    abbreviation = "PLN";
                    break;
                case "romanian leu":
                    abbreviation = "RON";
                    break;
                case "russian ruble":
                    abbreviation = "RUB";
                    break;
                case "swedish krona":
                    abbreviation = "SEK";
                    break;
                case "singapore dollar":
                    abbreviation = "SGD";
                    break;
                case "thai baht":
                    abbreviation = "THB";
                    break;
                case "turkish lira":
                    abbreviation = "TRY";
                    break;
                case "united states dollar":
                    abbreviation = "USD";
                    break;
                case "south african rand":
                    abbreviation = "ZAR";
                    break;
                case "euro":
                    abbreviation = "EUR";
                    break;
                case "new zealand dollar":
                    abbreviation = "NZD";
                    break;
                default:
                    abbreviation = "NZD";
                    break;
            }

            return abbreviation;
        }
    }
}
