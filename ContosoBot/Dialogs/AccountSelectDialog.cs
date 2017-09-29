using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class AccountSelectDialog : TypingReply, IDialog
    {
        private readonly IEnumerable<Account> _accounts;
        private readonly bool _saveGlobally;
        private readonly bool _suggestions;
        private readonly string _message;

        public AccountSelectDialog(bool suggestions = false, bool saveGlobally = true, string message = "Select account:")
        {
            // have to convert to a list, otherwise get a serialization error (?!)
            _accounts     = AccountDataController.Accounts.ToList();
            _saveGlobally = saveGlobally;
            _message      = message;
            _suggestions  = suggestions;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(PostTypingReply(context));

            var reply = context.MakeMessage();
            if (_suggestions)
            {
                reply.SuggestedActions = GetAccountSuggestions();
                reply.Text             = _message;
            }
            else
            {
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments      = GetAccountAttachments();

                if (!string.IsNullOrEmpty(_message))
                    await context.PostAsync(_message);
            }

            reply.InputHint = InputHints.ExpectingInput;

            //await context.PostAsync($"{_currentAccount.Name} is the current active account. Select new account to work with:");

            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private SuggestedActions GetAccountSuggestions()
        {
            var suggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>()
            };

            foreach (var account in _accounts)
            {
                suggestedActions.Actions.Add(new CardAction { Title = account.Name, Type = ActionTypes.ImBack, Value = account.Name });
            }

            return suggestedActions;
        }

        public IList<Attachment> GetAccountAttachments()
        {
            var result = new List<Attachment>();

            foreach (var account in _accounts)
            {
                result.Add(
                    new ThumbnailCard()
                    {
                        Title = account.Name,
                        Subtitle = account.Number,
                        //Text   = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}",
                        Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, "Select", value: account.Name) } //return account name for Slack and Skype compatibility
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

                //await context.PostAsync($"{selectedAccount.Name} selected");
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