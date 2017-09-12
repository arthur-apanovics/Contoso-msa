using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot.Dialogs
{
    [Serializable]
    [LuisModel("43bc854b-e59d-4a85-876a-728c5f40861a", "26bb2addf9a440209b8e296b6b777c7d")]
    public class LuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't understand that");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            var message = result.Query;
            await context.PostAsync($"{message} to you too!");
        }

        [LuisIntent("QueryAccounts")]
        public async Task QueryAccounts(IDialogContext context, LuisResult result)
        {
            context.Call(new AccountQueryDialog(), Callback);
        }

        [LuisIntent("QueryTransactionsByDates")]
        public async Task QueryTransactionsByDates(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Just a moment, getting data...");
            context.Call(new TransactionsQueryDialog(result), Callback);
        }

        [LuisIntent("Transfer")]
        public async Task TransferFunds(IDialogContext context, LuisResult result)
        {
            context.Call(new TransferDialog(result), Callback);
        }

        [LuisIntent("QueryExchangeRate")]
        public async Task QueryExchangeRate(IDialogContext context, LuisResult result)
        {
            context.Call(new ExchangeRateDialog(result), Callback);
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            //TODO: make username accessible from a static class
            await context.PostAsync("What would you like to do next" + (context.UserData.TryGetValue(DataStrings.Name, out string userName) ? $", {userName}?" : "?"));
            context.Wait(MessageReceived);
        }
    }
}