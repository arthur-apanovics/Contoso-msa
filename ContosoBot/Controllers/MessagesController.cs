﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Text == ".delete")
                {
                    activity.Type = ActivityTypes.DeleteUserData;
                    HandleSystemMessage(activity);
                }
                else
                { 
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity isTypingReply = activity.CreateReply();
                    isTypingReply.Type = ActivityTypes.Typing;
                    isTypingReply.InputHint = InputHints.IgnoringInput;
                    await connector.Conversations.ReplyToActivityAsync(isTypingReply);

                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                message.GetStateClient().BotState
                    .DeleteStateForUserWithHttpMessagesAsync(message.ChannelId, message.From.Id);

                var client = new ConnectorClient(new Uri(message.ServiceUrl));
                var clearMsg = message.CreateReply();
                clearMsg.Text = $"Resetting everything for conversation: {message.Conversation.Id}";
                client.Conversations.SendToConversationAsync(clearMsg);
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    var reply = message.CreateReply("Contoso&trade; Bank Assistant Bot v0.6");

                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}