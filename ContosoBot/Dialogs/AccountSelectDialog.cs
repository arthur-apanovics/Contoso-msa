using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using ContosoBot.Forms;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class AccountSelectDialog : IDialog
    {

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Just a moment, getting your accounts data...");

            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetAcocuntAttachments();

            await context.PostAsync("Please select account to work with:");

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
            //TODO: Validation & Typing support
            var message = await result;
            var deserializedAccount = JsonConvert.DeserializeObject<Account>(message.Value.ToString());
            context.Done(deserializedAccount);
        }
    }
}