using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs.Scorables
{
    public class AccountScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogStack _stack;

        public AccountScorable(IDialogStack stack)
        {
            SetField.NotNull(out _stack, nameof(stack), stack);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return state != null && state == "accountScorable-triggered" ? 1 : 0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null && state == "accountScorable-triggered";
        }

        protected override Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;
            var dialog = new AccountSelectDialog();
            var interruption = dialog.Void(_stack);
            _stack.Call(interruption, null);
            return Task.CompletedTask;
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item.AsMessageActivity();
            if (message == null)
                return null;

            var messageText = message.Text;

            return messageText == "select account" ? "accountScorable-triggered" : null;
        }
    }
}