using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronic;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class AccountSelectDialog : IDialog
    {
        private readonly IEnumerable<Account> _accounts;
        private Account _currentAccount;
        private bool _saveGlobally;

        public AccountSelectDialog(bool saveGlobally = true)
        {
            // have to convert to a list, otherwise get a serialization error (?!)
            _accounts = AccountDataController.Accounts.ToList();
            _saveGlobally = saveGlobally;
        }

        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Just a moment, getting your accounts data...");

            var reply              = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.InputHint        = InputHints.ExpectingInput;
            reply.Attachments      = GetAccountAttachments();

            if (!context.ConversationData.TryGetValue(DataStrings.ActiveAccount, out _currentAccount) || !_saveGlobally)
                await context.PostAsync("Select account:");
            else
                await context.PostAsync($"**{_currentAccount.Name}** is the current active account. Select new account to work with:");

            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        public IList<Attachment> GetAccountAttachments()
        {
            var result = new List<Attachment>();

            foreach (var account in _accounts)
            {
                result.Add(
                    new ThumbnailCard()
                    {
                        Title    = account.Name,
                        Subtitle = account.Number,
                        //Text   = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}",
                        Buttons  = new List<CardAction>() { new CardAction(ActionTypes.PostBack, "Select", value: account.Name) } //return account name for Slack and Skype compatibility
                    }
                    .ToAttachment()
                );
            }

            return result;
        }

        //TODO: Validation; Too many if statements
        //DONE: Typing support
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            Account selectedAccount = null;

            //Check message and respond with action
            if (message.Value != null)
            {
                try
                {
                    selectedAccount = JsonConvert.DeserializeObject<Account>(message.Value.ToString());
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else if (!string.IsNullOrEmpty(message.Text))
            {
                //Check if user typed name of account instead.
                foreach (var account in _accounts)
                {
                    if (string.Equals(account.Name, message.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        selectedAccount = account;
                        break;
                    }

                    //TODO: Use scorables to handle 'quit'
                    if (message.Text == "quit")
                    {
                        context.Fail(new Exception("User quit"));
                        return;
                    }
                }
            }

            if (selectedAccount != null)
            {
                if (_saveGlobally)
                    context.ConversationData.SetValue(DataStrings.ActiveAccount, selectedAccount);

                await context.PostAsync($"{selectedAccount.Name} selected");
                context.Done(selectedAccount);
            }
            else
            {
                var options = string.Empty;

                foreach (var account in _accounts)
                {
                    options += $"* {account.Name}  \n";
                }

                await context.PostAsync($"Sorry, no account with that name. Options are:\n{options}\n\nPlease try again");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}