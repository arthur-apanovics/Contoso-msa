using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ContosoData;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot.Dialogs
{
    public class ExchangeRateDialog : IDialog
    {
        private EntityProps _entityProps;

        public ExchangeRateDialog(LuisResult luisResult)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            var currencyOperations = new CurrencyOperations();

            if (_entityProps.MoneyCurrency.Count >= 2)
            {
                var baseCurrency   = _entityProps.MoneyCurrency[0];
                var targetCurrency = _entityProps.MoneyCurrency[1];

                if (_entityProps.MoneyAmount != 0)
                {
                    var amount          = _entityProps.MoneyAmount;
                    var convertedResult = currencyOperations.ConvertCurrency(baseCurrency, targetCurrency, amount);

                    await context.PostAsync(
                        $"{amount} {baseCurrency}'s is equal to {convertedResult} {targetCurrency}'s");

                    context.Done(true);
                }
                else
                {
                    var rate = currencyOperations.GetExchangeRate(baseCurrency, targetCurrency);
                    await PrintResults(context, rate);
                }
            }
            else if (_entityProps.MoneyCurrency.Count == 1)
            {
                var baseCurrency = _entityProps.MoneyCurrency[0];
                var rate         = currencyOperations.GetExchangeRate(baseCurrency);
                await PrintResults(context, rate);
            }
            else
            {
                //TODO: Currency form
            }
        }

        private async Task PrintResults(IDialogContext context, ExchangeRate rates)
        {
            var result = new StringBuilder();

            result.Append($"1 {rates.Base} equals:  \n");

            foreach (var rate in rates.Rates.GetType().GetProperties())
            {
                var propValue = rate.GetValue(rates.Rates);
                if ((float) propValue == 0) continue; 

                result.Append($"{propValue} {rate.Name}  \n");
            }

            await context.PostAsync(result.ToString());
        }
    }
}