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
            context.Call(new AccountSelectDialog(), ResumeAfterAccountSelectDialog);
            return Task.CompletedTask;
        }

        private async Task ResumeAfterAccountSelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            var selection = await result;
            _selectedAccount = selection as Account;

            //check if date has been specified
            if (!_entityProps.DateRange.Start.HasValue)
            {
                var transactionQueryBuilderDialog = FormDialog.FromForm(BuildTransactionRangeForm, FormOptions.PromptInStart);
                context.Call(transactionQueryBuilderDialog, PerformTransactionQuery);
            }
            else
            {
                await PerformTransactionQuery(context, result);
            }

        }

        private IForm<TransactionHistoryRangeQuery> BuildTransactionRangeForm()
        {
            OnCompletionAsyncDelegate<TransactionHistoryRangeQuery> processQuery = async (context, state) =>
            {
                _entityProps.DateRange.Start = state.DateStart;
                _entityProps.DateRange.End = state.DateEnd;
            };

            return new FormBuilder<TransactionHistoryRangeQuery>()
                .Field(nameof(TransactionHistoryRangeQuery.DateStart))
                .AddRemainingFields()
                .OnCompletion(processQuery)
                .Build();
        }

        private async Task PerformTransactionQuery (IDialogContext context, IAwaitable<object> result)
        {


            //var queriedTransactions = AccountDataController.GetTransactionRangeByDate(_selectedAccount, query.DateStart, query.DateEnd);

            string output = String.Empty;

            //foreach (var transaction in queriedTransactions)
            //{
            //    output +=
            //        $"\n * {transaction.Amount:C} **to** {transaction.RecepientName}, **on** {transaction.DateTime:d}, '{transaction.Description}'";
            //}

             context.PostAsync(String.IsNullOrWhiteSpace(output) ? "No transactions found" : output);

            context.Done(true);
        }
    }
}