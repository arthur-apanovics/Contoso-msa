using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    public class NameChangeDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            if (!context.UserData.TryGetValue(DataStrings.Name, out string userName))
                await context.PostAsync("Please input your name");
            else
                await context.PostAsync($"OK {userName}, what is your preferred name?");

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(message.Text);

            context.UserData.SetValue(DataStrings.Name, name);

            await context.PostAsync($"OK {name}, your name has been updated.");

            context.Done(true);
        }
    }
}