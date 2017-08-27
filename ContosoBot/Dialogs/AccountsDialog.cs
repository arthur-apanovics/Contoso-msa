using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Forms;
using ContosoData.Contollers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class AccountsDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Just a moment, getting your accounts...");

            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetAcocuntAttachments();

            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        private IList<Attachment> GetAcocuntAttachments()
        {
            var userAccounts = new AccountDataController().GetAccounts();

            var result = new List<Attachment>();

            foreach (var account in userAccounts)
            {
                result.Add(
                    new ThumbnailCard()
                    {
                        Title = account.Name,
                        Subtitle = account.Number,
                        Text = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}",
                        Buttons = new List<CardAction>() { new CardAction(ActionTypes.PostBack, "Select", value: account) }
                    }
                    .ToAttachment()
                );
            }

            return result;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            context.Wait(MessageReceivedAsync);
        }
    }
}