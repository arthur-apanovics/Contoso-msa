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
                _signedIn = true;

                context.Call(new LoginDialog(), MessageReceivedAsync);
            }
            else
            {
                context.Call(new LuisDialog(), ResumeAfterLuis);
            }
        }

        private Task ResumeAfterLuis(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException("Exited Luis Dialog, back in root.");
        }
    }
}