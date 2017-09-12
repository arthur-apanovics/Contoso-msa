using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace ContosoBot.Dialogs
{
    public class HelpDialog : IDialog<object>
    {
        private const string HelpMessage = "Here's what I can help you with:  \n" +
                                     "* Checking account information  \n" +
                                     "* Viewing transaction history  \n" +
                                     "* Performing internal fund transfers" +
                                     "* Converting and viewing currency rates  \n" +
                                     "\nSome tips:\n\n" +
                                     "* Type '*select account*' to change the active account  \n" +
                                     "* Type '*change name*' to fix a typo in your name  \n" +
                                     "* Type '*quit*' to exit from a dialog  \n" +
                                     "* Even though my heart is made of transistors, you can still talk to me like you would to a human";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(HelpMessage);

            context.Done(true);
        }
    }
}