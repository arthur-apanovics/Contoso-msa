using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //private const string HelpMessage = "Here's what I can help you with:" +
        //                                   "\n * Checking account information" +
        //                                   "\n * Making payments and transfers" +
        //                                   "\n * Viewing payment and transfer history";
        private const string HelpMessage = "Here's what's working right now:" +
                                           "\n * Checking account information" +
                                           "\n * Viewing transaction history";
        private string _userName;
        private bool _userWelcomed;


        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (!_userWelcomed)
            {
                if (!context.UserData.TryGetValue("Name", out _userName))
                {
                    await context.PostAsync("Welcome. I am your Contoso Banking assistant.");
                    PromptDialog.Text(context, ResumeAfterPrompt, "Before we get started, could you please tell me your name?");
                }
                else
                {
                    await context.PostAsync($"**Welcome back, {_userName}**. {HelpMessage}");
                    _userWelcomed = true;

                    context.Call(new LuisDialog(), ResumeAfterLuis);
                }
            }
        }

        private Task ResumeAfterLuis(IDialogContext context, IAwaitable<Object> result)
        {
            throw new NotImplementedException("Exited Luis Dialog, back in root.");
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            var userName = await result;
            _userWelcomed = true;

            await context.PostAsync($"**Hello, {userName}!** {HelpMessage}");

            context.UserData.SetValue("Name", userName);

            context.Call(new LuisDialog(), ResumeAfterLuis);
        }
    }
}