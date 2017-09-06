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
        private EntityProps _entityProps;

        public TransactionsQueryDialog(LuisResult luisResult = null)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (!context.ConversationData.TryGetValue("Account", out _selectedAccount))
                context.Call(new AccountSelectDialog(), PerformTransactionQuery);
            else
                await context.PostAsync($"Using {_selectedAccount.Name} account. Type 'select account' to change");
                await PerformTransactionQuery(context, new AwaitableFromItem<object>(_selectedAccount));
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