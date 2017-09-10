using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ContosoData.Contollers
{
    public class ExchangeRateApiController
    {
        public string GetRates(string currency = "NZD")
        {
            string baseUrl = "http://api.fixer.io/latest?base=";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + currency);
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
                    // log error
                    String errorText = reader.ReadToEnd();
                }
                throw;
            }
        }
    }
}
