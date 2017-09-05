using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Models;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class TransactionsQueryDialog : IDialog
    {
        private Account _selectedAccount;
        private EntityProps _entityProps;

        public TransactionsQueryDialog(LuisResult luisResult = null)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Call(new AccountSelectDialog(), PerformTransactionQuery);
            return Task.CompletedTask;
        }

        private async Task PerformTransactionQuery (IDialogContext context, IAwaitable<object> result)
        {
            var selection = await result;
            _selectedAccount = selection as Account;

            //check if enough info was supplied to create an entity collection for query
            if (_entityProps == null)
                await context.PostAsync("Sorry, you didn't supply enough information. Showing 5 latest transactions");

            var queryResult = 
                AccountDataController.GetTransactionsFromEntities(_selectedAccount, _entityProps);

            string output = String.Empty;

            foreach (var transaction in queryResult)
            {
                output +=
                    $"\n * {transaction.DateTime:d}: {transaction.Amount:C} transferred to {transaction.RecepientName}";
            }

            await context.PostAsync(String.IsNullOrWhiteSpace(output) ? "Sorry, no transactions found" : output);

            context.Done(true);
        }
    }
}