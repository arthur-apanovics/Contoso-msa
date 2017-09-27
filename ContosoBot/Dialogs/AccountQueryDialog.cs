using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Activity = Microsoft.Bot.Connector.Activity;

namespace ContosoBot.Dialogs
{
    public class AccountQueryDialog : TypingReply, IDialog
    {
        private EntityProps _entityProps;

        public AccountQueryDialog(LuisResult result)
        {
            _entityProps = new EntityAssigner().AssignEntities(result);
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (_entityProps.Account == null)
                await ShowAccountOptions(context);

            else if (!string.IsNullOrEmpty(_entityProps.Account.Name))
                await ShowAccountBalance(context, null);

        }

        private async Task ShowAccountOptions(IDialogContext context)
        {
            await context.PostAsync(PostTypingReply(context));

            var suggestionMessage = context.MakeMessage();
            suggestionMessage.Text = "Great, please choose an account to view";

            suggestionMessage.SuggestedActions = new SuggestedActions() { Actions = new List<CardAction>() };

            foreach (var account in AccountDataController.Accounts)
            {
                suggestionMessage.SuggestedActions.Actions.Add(
                    new CardAction { Title = account.Name, Type = ActionTypes.ImBack, Value = account.Name }
                );
            }
            suggestionMessage.SuggestedActions.Actions.Add(
                    new CardAction { Title = "All", Type = ActionTypes.ImBack, Value = "All" }
                );

            await context.PostAsync(suggestionMessage);

            context.Wait(ShowAccountBalance);
        }

        private async Task ShowAccountBalance(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await context.PostAsync(PostTypingReply(context));

            IMessageActivity message;

            // Can't get this to work...
            //IMessageActivity message = await result ?? new Activity(type: "message", text: _entityProps.Account.Name);

            // Using if statement instead
            if (result != null)
                message = await result;
            else
                message = new Activity(type: "message", text: _entityProps.Account.Name);


            if (VerifyAccountName(message?.Text))
            {
                var reply              = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.List;
                reply.Attachments      = GetAccountAttachments(message?.Text);

                await context.PostAsync(reply);
            }

            context.Done(true);
        }

        private bool VerifyAccountName(string accountName)
        {
            foreach (var account in AccountDataController.Accounts)
            {
                if (account.Name == accountName || accountName.Equals("All", StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        private IList<Attachment> GetAccountAttachments(string accountName)
        {
            var userAccounts = accountName.Equals("All", StringComparison.CurrentCultureIgnoreCase) ? 
                AccountDataController.Accounts :  AccountDataController.Accounts.Where(a => a.Name == accountName);

            var result = new List<Attachment>();

                foreach (var account in userAccounts)
                {
                    result.Add(
                        new ThumbnailCard()
                            {
                                Title    = account.Name,
                                Subtitle = account.Number,
                                Text     = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}"
                            }
                            .ToAttachment()
                    );
                }

            return result;
        }
    }
}