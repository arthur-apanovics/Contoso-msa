using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace ContosoBot.Dialogs
{
    public class HelpDialog : IDialog<object>
    {
        private const string HelpMessage = "Here's what I can help you with:  \n" +
                                     "* Checking account information  \n" +
                                     "* Viewing transaction history  \n" +
                                     "* Performing internal fund transfers  \n" +
                                     "* Converting and viewing currency rates  \n" +
                                     "\nExample questions you can try:\n\n" +
                                     "* What is my account balance?  \n" +
                                     "* Show me the last transaction to Juniper Networks from 15th April 2017 to 01/06/2017, that is over two hundred dollars  \n" +
                                     "* Transfer 250$ to my savings account \n" +
                                     "* Show me the rate of Russian ruble and South Korean won  \n" +
                                     "Don't forget that you can use 'quit' to exit from a dialog  \n" +
                                     "Even though my heart is made of transistors, you can still talk to me like you would to a human";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(HelpMessage);

            context.Done(true);
        }
    }
}