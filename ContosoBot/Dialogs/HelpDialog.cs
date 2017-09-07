using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    public class HelpDialog : IDialog<object>
    {
        private const string HelpMessage = "Here's what I can help you with:  \n" +
                                     "* Checking account information  \n" +
                                     "* Viewing transaction history  \n" +
                                     "\nSome tips:\n\n" +
                                     "* Type 'select account' to change the active account  \n" +
                                     "* Even though my heart is made of transistors, you can still talk to me like you would to a human";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(HelpMessage);

            context.Done(true);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                context.Done<object>(null);
            }
            else
            {
                context.Fail(new Exception("Message was not a string or was an empty string."));
            }
        }
    }
}