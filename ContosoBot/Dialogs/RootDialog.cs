using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string AccountsOption = "My Account";
        private const string PaymentsOption = "Payments";
        private const string SupportOption  = "Support";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome. I am your Contoso Banking assistant. What can I help you with today?");
            ShowOptions(context);
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context,
                OnSelectionMade,
                new List<string>(){ AccountsOption, PaymentsOption, SupportOption },
                "Available categories",
                "Sorry, I didn't get that. Please choose one of the available categories.");
        }

        private async Task OnSelectionMade(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string selection = await result;

                switch (selection)
                {
                    case AccountsOption:
                        context.Call(new AccountsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case PaymentsOption:
                        context.Call(new PaymentsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case SupportOption:
                        context.Call(new FaqDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync("Sorry, too many failed attemps. Let's start over.");

                context.Wait(MessageReceivedAsync);
            }
        }

        //TODO: Check this right here.
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"!!! Failed with message: {ex.Message}. Starting over.");
                await MessageReceivedAsync(context, result);
                //context.Wait(MessageReceivedAsync);
            }
        }
    }
}