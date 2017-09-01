using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Models;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class TransactionsQueryDialog : IDialog
    {
        private Account _selectedAccount;
        private IList<EntityRecommendation> _luisEntities;

        // LUIS built in entity to resolve people and company names.
        private string _encyclopedia;
        private float _money;
        private int _ordinal;
        private DateRange _dateRange;


        public TransactionsQueryDialog(IList<EntityRecommendation> luisEntities = null)
        {
            _luisEntities = luisEntities;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Call(new AccountSelectDialog(), ResumeAfterAccountSelectDialog);

            foreach (var entityRecommendation in _luisEntities)
            {
                //handle encycplopedia seperately due to multiple recommendations per encyclopedia entity
                if (entityRecommendation.Type.Contains("builtin.encyclopedia") && string.IsNullOrEmpty(_encyclopedia))
                {
                    _encyclopedia = entityRecommendation.Entity;
                    return;
                }
                switch (entityRecommendation.Type)
                {
                    case "builtin.encyclopedia":

                        _encyclopedia = entityRecommendation.Entity;
                        break;

                    case "builtin.currency":
                        var resolved = entityRecommendation.Resolution
                            .FirstOrDefault(r => r.Key == "value")
                            .Value
                            .ToString();

                        float.TryParse(resolved, out _money);
                        break;

                    case "builtin.datetimeV2.daterange":
                        var resolutionJson = entityRecommendation.Resolution
                            .FirstOrDefault(r => r.Key == "values")
                            .Value
                            .ToString()
                            .Trim('[', ']');

                        _dateRange =
                            JsonConvert.DeserializeObject<DateRange>(resolutionJson);
                        break;
                }
            }
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
            var queriedTransactions = AccountDataController.GetTransactionRangeByDate(_selectedAccount, query.DateStart, query.DateEnd);

            string output = String.Empty;

            foreach (var transaction in queriedTransactions)
            {
                output +=
                    $"\n * {transaction.Amount:C} **to** {transaction.RecepientName}, **on** {transaction.DateTime:d}, '{transaction.Description}'";
            }

            await context.PostAsync(String.IsNullOrWhiteSpace(output) ? "No transactions found" : output);

            context.Done(true);
        }
    }
}