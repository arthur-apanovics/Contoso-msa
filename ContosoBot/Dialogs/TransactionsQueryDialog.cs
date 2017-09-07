using System;
using System.Threading.Tasks;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class TransactionsQueryDialog : IDialog
    {
        private Account _selectedAccount;
        private readonly EntityProps _entityProps;

        public TransactionsQueryDialog(LuisResult luisResult = null)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (!context.ConversationData.TryGetValue(DataStrings.ActiveAccount, out _selectedAccount))
                context.Call(new AccountSelectDialog(), PerformTransactionQuery);
            else
            {
                await context.PostAsync($"Using **{_selectedAccount.Name}** account, type *'select account'* to change");
                await PerformTransactionQuery(context, new AwaitableFromItem<object>(_selectedAccount));
            }
        }

        private async Task PerformTransactionQuery (IDialogContext context, IAwaitable<object> result)
        {
            var selection    = await result;
            _selectedAccount = selection as Account;
            string output    = String.Empty;

            var queryResult = 
                AccountDataController.GetTransactionsFromEntities(_selectedAccount, _entityProps);

            //check if enough info was supplied to create an entity collection for query
            output += _entityProps == null ? "Sorry, you didn't supply enough information, showing latest 5 transactions:" : "";

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