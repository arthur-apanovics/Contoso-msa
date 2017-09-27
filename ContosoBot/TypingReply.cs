using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot
{
    [Serializable]
    public abstract class TypingReply
    {
        public IMessageActivity PostTypingReply(IDialogContext context)
        {
            //create a typing reply
            var wait = context.MakeMessage();
            wait.Type = ActivityTypes.Typing;
            return wait;
        }
    }
}