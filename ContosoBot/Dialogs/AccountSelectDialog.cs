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
    public class AccountSelectDialog : IDialog
    {
        private readonly IEnumerable<Account> _accounts;

        public AccountSelectDialog()
        {
            // have to convert to a list, otherwise get a serialization error (?!)
            _accounts = AccountDataController.Accounts.ToList();
        }

        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Just a moment, getting your accounts data...");

            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.InputHint = InputHints.ExpectingInput;
            reply.Attachments = GetAccountAttachments();

            await context.PostAsync("Please select account to work with:");
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        private IList<Attachment> GetAccountAttachments()
        {
            var result = new List<Attachment>();

            foreach (var account in _accounts)
            {
                result.Add(
                    new ThumbnailCard()
                    {
                        Title = account.Name,
                        Subtitle = account.Number,
                        //Text = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}",
                        Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, "Select", value: account) }
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
            else if(!string.IsNullOrEmpty(message.Text))
            {
                //Check if user typed name of account instead.
                foreach (var account in _accounts)
                {
                    if (string.Equals(account.Name, message.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        selectedAccount = account;
                        break;
                    }
                }
            }

            if (message.Text == "quit")
            {
                context.Fail(new Exception("User quit"));
            }
            else if (selectedAccount != null)
            {
                await context.PostAsync($"{selectedAccount.Name} selected");
                context.Done(selectedAccount);
            }
            else
            {
                await context.PostAsync("Sorry, no account with that name. Please try again");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}