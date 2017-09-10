using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoData.Model
{
    class ExchangeRate
    {
            public string Base { get; set; }
            public string Date { get; set; }
            public Rates Rates { get; set; }
    }

    public class Rates
        {
            public float AUD { get; set; }
            public float BGN { get; set; }
            public float BRL { get; set; }
            public float CAD { get; set; }
            public float CHF { get; set; }
            public float CNY { get; set; }
            public float CZK { get; set; }
            public float DKK { get; set; }
            public float GBP { get; set; }
            public float HKD { get; set; }
            public float HRK { get; set; }
            public float HUF { get; set; }
            public float IDR { get; set; }
            public float ILS { get; set; }
            public float INR { get; set; }
            public float JPY { get; set; }
            public float KRW { get; set; }
            public float MXN { get; set; }
            public float MYR { get; set; }
            public float NOK { get; set; }
            public float PHP { get; set; }
            public float PLN { get; set; }
            public float RON { get; set; }
            public float RUB { get; set; }
            public float SEK { get; set; }
            public float SGD { get; set; }
            public float THB { get; set; }
            public float TRY { get; set; }
            public float USD { get; set; }
            public float ZAR { get; set; }
            public float EUR { get; set; }
        }

    class CurrencyOperations
    {
        private string GetCurrencyAbbreviation(string currency)
        {
            var abbreviation = string.Empty;

            switch (currency)
            {
                case "Australian dollar":
                    abbreviation = "AUD";
                    break;
                case "Bulgarian lev":
                    abbreviation = "BGN";
                    break;
                case "Brazilian real":
                    abbreviation = "BRL";
                    break;
                case "Canadian dollar":
                    abbreviation = "CAD";
                    break;
                case "Swiss franc":
                    abbreviation = "CHF";
                    break;
                case "Chinese yuan":
                    abbreviation = "CNY";
                    break;
                case "Czech koruna":
                    abbreviation = "CZK";
                    break;
                case "Danish krone":
                    abbreviation = "DKK";
                    break;
                case "British pound":
                    abbreviation = "GBP";
                    break;
                case "Hong Kong dollar":
                    abbreviation = "HKD";
                    break;
                case "Croatian kuna":
                    abbreviation = "HRK";
                    break;
                case "Hungarian forint":
                    abbreviation = "HUF";
                    break;
                case "Indonesian rupiah":
                    abbreviation = "IDR";
                    break;
                case "Israeli new shekel":
                    abbreviation = "ILS";
                    break;
                case "Indian rupee":
                    abbreviation = "INR";
                    break;
                case "Japanese yen":
                    abbreviation = "JPY";
                    break;
                case "South Korean won":
                    abbreviation = "KRW";
                    break;
                case "Mexican peso":
                    abbreviation = "MXN";
                    break;
                case "Malaysian ringgit":
                    abbreviation = "MYR";
                    break;
                case "Norwegian krone":
                    abbreviation = "NOK";
                    break;
                case "Philippine peso":
                    abbreviation = "PHP";
                    break;
                case "Polish złoty":
                    abbreviation = "PLN";
                    break;
                case "Romanian leu":
                    abbreviation = "RON";
                    break;
                case "Russian ruble":
                    abbreviation = "RUB";
                    break;
                case "Swedish krona":
                    abbreviation = "SEK";
                    break;
                case "Singapore dollar":
                    abbreviation = "SGD";
                    break;
                case "Thai baht":
                    abbreviation = "THB";
                    break;
                case "":
                    abbreviation = "TRY";
                    break;
                case "United States dollar":
                    abbreviation = "USD";
                    break;
                case "South African rand":
                    abbreviation = "ZAR";
                    break;
                case "Euro":
                    abbreviation = "EUR";
                    break;
                default:
                    abbreviation = "NZD";
                    break;
            }

            return abbreviation;
        }
    }
}
