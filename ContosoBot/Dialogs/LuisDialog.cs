using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    [Serializable]
    [LuisModel("43bc854b-e59d-4a85-876a-728c5f40861a", "26bb2addf9a440209b8e296b6b777c7d")]
    public class LuisDialog : LuisDialog<object>
    {
        public override async Task StartAsync(IDialogContext context)
        {
            await PostSuggestions(context);
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I didn't understand that");
            await PostSuggestions(context);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            context.Call(new HelpDialog(), Callback);
        }

        [LuisIntent("Greeting")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            var message = result.Query;
            await context.PostAsync($"{message} to you too!");
            await PostSuggestions(context);
        }

        [LuisIntent("QueryAccounts")]
        public async Task QueryAccounts(IDialogContext context, LuisResult result)
        {
            context.Call(new AccountQueryDialog(result), Callback);
        }

        [LuisIntent("QueryTransactionsByDates")]
        public async Task QueryTransactionsByDates(IDialogContext context, LuisResult result)
        {
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
            await PostSuggestions(context);
            context.Wait(MessageReceived);
        }

        private async Task PostSuggestions(IDialogContext context)
        {
            var suggestionMessage = context.MakeMessage();
            suggestionMessage.Text = $"What can I help you with, {context.Activity.From.Name}? Type or select an option";
            suggestionMessage.InputHint = InputHints.ExpectingInput;

            suggestionMessage.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction{ Title = "Account balance", Type=ActionTypes.ImBack, Value="Account balance" },
                    new CardAction{ Title = "Transaction History", Type=ActionTypes.ImBack, Value="Transactions" },
                    new CardAction{ Title = "Internal Transfer", Type=ActionTypes.ImBack, Value="Transfer" },
                    new CardAction{ Title = "Show Help", Type=ActionTypes.ImBack, Value="Help me figure out" }
                }
            };

            await context.PostAsync(suggestionMessage);
        }
    }
}