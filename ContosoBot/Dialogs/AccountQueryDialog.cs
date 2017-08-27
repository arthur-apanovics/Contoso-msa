﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoData.Contollers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    public class AccountQueryDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.List;
            reply.Attachments = GetAcocuntAttachments();

            await context.PostAsync(reply);
            
            context.Done("");
        }

        private IList<Attachment> GetAcocuntAttachments()
        {
            var userAccounts = new AccountDataController().GetAccounts();

            var result = new List<Attachment>();

            foreach (var account in userAccounts)
            {
                result.Add(
                    new ThumbnailCard()
                        {
                            Title = account.Name,
                            Subtitle = account.Number,
                            Text = $"Type: {account.Type}, Overdraft limit: {account.OverdraftLimit:C}, Balance: {account.Balance:C}"
                        }
                        .ToAttachment()
                );
            }

            return result;
        }
    }
}