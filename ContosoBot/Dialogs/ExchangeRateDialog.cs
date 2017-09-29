using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContosoData;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    public class ExchangeRateDialog : IDialog
    {
        private EntityProps _entityProps;
        private string _sourceCurrency;
        private string _targetCurrency;
        private float _amount;

        public ExchangeRateDialog(LuisResult luisResult)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            var currencyOperations = new CurrencyOperations();

            if (_entityProps.MoneyCurrency.Count >= 2)
            {
                var baseCurrency = _entityProps.MoneyCurrency[0];
                var targetCurrency = _entityProps.MoneyCurrency[1];

                if (_entityProps.MoneyAmount != 0)
                {
                    var amount = _entityProps.MoneyAmount;
                    var convertedResult = currencyOperations.ConvertCurrency(baseCurrency, targetCurrency, amount);

                    await context.PostAsync(
                        $"{amount} {baseCurrency}'s are equal to {convertedResult} {targetCurrency}'s");

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
                var rate = currencyOperations.GetExchangeRate(baseCurrency);
                await PrintResults(context, rate);
            }
            else
            {
                await context.PostAsync(GetSuggestedActions(context, "Bienvenu! View the rate or convert?"));
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text.ToLower())
            {
                case "view rate":
                    await context.PostAsync(CurrencyPicker(context, "Select a currency, s'il vous plaît"));
                    context.Wait(SetSourceAndPrint);
                    break;
                case "convert":
                    await context.PostAsync(CurrencyPicker(context, "Bien. What's the source currency?"));
                    context.Wait(ExchangeSourceCurrencySet);
                    break;
                default:
                    await context.PostAsync(GetSuggestedActions(context, "Quoi? Please try again"));
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        private async Task SetSourceAndPrint(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            _sourceCurrency = message.Text;

            var rates = new CurrencyOperations().GetExchangeRate(_sourceCurrency);
            await PrintResults(context, rates);
        }

        private IMessageActivity GetSuggestedActions(IDialogContext context, string message)
        {
            var suggestionMessage = context.MakeMessage();
            suggestionMessage.Text = message;
            suggestionMessage.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "View Rate", Type = ActionTypes.ImBack, Value = "View Rate"},
                    new CardAction(){ Title = "Convert", Type = ActionTypes.ImBack, Value = "Convert"}
                }
            };

            return suggestionMessage;
        }

        private async Task ExchangeSourceCurrencySet(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            _sourceCurrency = message.Text;

            await context.PostAsync(CurrencyPicker(context, "C'est manifique! Now the target currency"));
            context.Wait(ExchangeTargetCrrencySet);
        }

        private async Task ExchangeTargetCrrencySet(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message     = await result;
            _targetCurrency = message.Text;

            PromptDialog.Number(context, ResumeAfterAmountDialog, $"Absolument incroyable! How many {_sourceCurrency} would you like to convert?");
        }

        private async Task ResumeAfterAmountDialog(IDialogContext context, IAwaitable<double> result)
        {
            var message = await result;
            _amount     = (float)message;

            var convertedAmount = new CurrencyOperations().ConvertCurrency(_sourceCurrency, _targetCurrency, _amount);

            var resultCard = context.MakeMessage();
            resultCard.Attachments.Add(
                new ThumbnailCard()
                    {
                        Text = $"{_amount} {_sourceCurrency} is worth {convertedAmount} {_targetCurrency}",
                    }
                    .ToAttachment());

            await context.PostAsync(resultCard);

            context.Done(true);
        }

        private IMessageActivity CurrencyPicker(IDialogContext context, string message)
        {
            var suggestionMessage = context.MakeMessage();
            suggestionMessage.Text = message;
            suggestionMessage.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
            };

            var rateProperties = typeof(Rates).GetProperties();

            foreach (var currency in rateProperties)
            {
                if (currency.Name == _sourceCurrency) continue;
                var item = new CardAction() { Title = currency.Name, Type = ActionTypes.ImBack, Value = currency.Name };
                suggestionMessage.SuggestedActions.Actions.Add(item);
            }

            return suggestionMessage;
        }

        private async Task PrintResults(IDialogContext context, ExchangeRate rates)
        {
            var result = new StringBuilder();

            result.Append($"1 {rates.Base} equals:  \n");

            //IMessageActivity result = context.MakeMessage();

            foreach (var rate in rates.Rates.GetType().GetProperties())
            {
                var propValue = rate.GetValue(rates.Rates);
                if ((float)propValue == 0) continue;

                //TODO: Build card for rate result
                //result.Attachments.Add(
                //    new ThumbnailCard()
                //    {
                //        Title = $"{rates.Base}{propValue}"
                //    });

                result.Append($"{propValue} {rate.Name}  \n");
            }

            await context.PostAsync(result.ToString());

            context.Done(true);
        }
    }
}