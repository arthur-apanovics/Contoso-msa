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
        private const string WelcomeMessage = "\n\nLet me know what you would like to do." +
                                              "\n\nHint - type '*help*' if you need more information.";
        private string _userName;
        private bool _userWelcomed;
        private bool _signedIn;
        
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            if (!_signedIn)
            {
                context.Call(new LoginDialog(), MessageReceivedAsync);

                _signedIn = true;
            }
            else if (!_userWelcomed)
            {
                //await context.PostAsync($"Welcome, {context.Activity.From.Name}");
                _userWelcomed = true;

                context.Call(new LuisDialog(), ResumeAfterLuis);

                //if (!context.UserData.TryGetValue("Name", out _userName))
                //{
                //    PromptDialog.Text(context, ResumeAfterPrompt, "Before we get started, could you please tell me your name?");
                //}
                //else
                //{
                //    await context.PostAsync($"**Welcome back, {_userName}**. {WelcomeMessage}");
                //    _userWelcomed = true;

                //    context.Call(new LuisDialog(), ResumeAfterLuis);
                //}
            }
        }

        private Task ResumeAfterLuis(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException("Exited Luis Dialog, back in root.");
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            var userName = await result;
            _userWelcomed = true;

            await context.PostAsync($"**Hello, {userName}!** {WelcomeMessage}");

            context.UserData.SetValue("Name", userName);

            context.Call(new LuisDialog(), ResumeAfterLuis);
        }
    }
}