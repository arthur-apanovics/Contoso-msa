using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Models;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class TransactionsQueryDialog : IDialog
    {
        private Account _selectedAccount;

        public async Task StartAsync(IDialogContext context)
        {
            context.Call(new AccountSelectDialog(), ResumeAfterAccountSelectDialog);
        }

        private async Task ResumeAfterAccountSelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            var selection = await result;
            _selectedAccount = selection as Account;

            var transactionQueryDialog = FormDialog.FromForm(BuildTransactionRangeForm, FormOptions.PromptInStart);
            context.Call(transactionQueryDialog, ResumeAfterTransactionQueryDialog);
        }

        private IForm<TransactionHistoryRangeQuery> BuildTransactionRangeForm()
        {
            OnCompletionAsyncDelegate<TransactionHistoryRangeQuery> processQuery = async (context, form) =>
            {
                await context.PostAsync($"Ok. Looking for transactions from {form.DateStart:d} to {form.DateEnd:d}, from account {_selectedAccount.Name}");
            };

            return new FormBuilder<TransactionHistoryRangeQuery>()
                .Field(nameof(TransactionHistoryRangeQuery.DateStart))
                .AddRemainingFields()
                .OnCompletion(processQuery)
                .Build();
        }

        private async Task ResumeAfterTransactionQueryDialog(IDialogContext context, IAwaitable<TransactionHistoryRangeQuery> result)
        {
            var query = await result;
            var queriedTransactions = new AccountDataController()
                .GetTransactionRangeByDate(_selectedAccount, query.DateStart, query.DateEnd);

            string output = String.Empty;

            foreach (var transaction in queriedTransactions)
            {
                output +=
                    $"\n * {transaction.Amount:C} to {transaction.RecepientName}, on {transaction.DateTime:d}, '{transaction.Description}'";
            }

            await context.PostAsync(String.IsNullOrWhiteSpace(output) ? "No transactions found" : output);

            context.Done(true);
        }
    }
}