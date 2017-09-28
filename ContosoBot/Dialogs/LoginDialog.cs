using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoData.Contollers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    public class LoginDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            var attachment = new SigninCard
                {
                    Text = "Welcome. Please sign-in to your Contoso Banking account by clicking on the button below",
                    Buttons = new List<CardAction> { new CardAction(
                        ActionTypes.PostBack,
                        title:"Super Secure Sign-in",
                        value: "http://example.com/",
                        image: "http://3.bp.blogspot.com/-frRd1nH1EVA/TZXoVdy-LyI/AAAAAAAAAjg/tuqe3oH6t8U/s320/Contoso%2Blogo.png") }
                }
                .ToAttachment();

            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Great! You have been signed in using SSO");
            context.Done(true);
        }
    }
}