using System;
using System.IO;
using System.Net;
using System.Text;

namespace ContosoData.Contollers
{
    /// <summary>
    /// Quote against a currency by setting the base parameter in request. By default NZD is used as the base currency.
    /// </summary>
    public class ExchangeRateController
    {
        public string GetRates(string baseCurrency = "NZD")
        {
            string baseUrl = "http://api.fixer.io/latest?base=";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + baseCurrency);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                }
                throw;
            }
        }

        /// <summary>
        /// Request specific exchange rates for two currencies.
        /// </summary>
        /// <param name="firstSymbol"></param>
        /// <param name="secondSymbol"></param>
        /// <returns></returns>
        public string GetRates(string firstSymbol, string secondSymbol)
        {
            string baseUrl = "http://api.fixer.io/latest?base=";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}{firstSymbol}&symbols={secondSymbol}");
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                }
                throw;
            }
        }
    }
}
