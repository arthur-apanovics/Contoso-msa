using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot.Dialogs
{
    [Serializable]
    [LuisModel("43bc854b-e59d-4a85-876a-728c5f40861a", "26bb2addf9a440209b8e296b6b777c7d")]
    public class LuisDialog : LuisDialog<Object>
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
            context.Call(new AccountsDialog(), Callback);
        }

        [LuisIntent("QueryTransactionsByDates")]
        public async Task QueryTransactionsByDates(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("QueryTransactionsByDates recognized!!!");
        }

        private async Task Callback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }
    }
}