using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ContosoBot.Models;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    //TODO: This class has been hacked together. Need to improve logic

    public class TransferDialog : IDialog
    {
        private EntityProps _entityProps;
        private Account _selectedAccount;

        public TransferDialog(LuisResult luisResult)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.ConversationData.TryGetValue(DataStrings.ActiveAccount, out _selectedAccount);

            //all info supplied ?
            if (_entityProps.Account != null && _entityProps.MoneyAmount != 0 &&
                _selectedAccount != null)
            {
                await ValidateAccounts(context);
            }
            else if (_entityProps.Account != null && _entityProps.MoneyAmount != 0 &&
                     _selectedAccount == null)
            {
                await context.PostAsync("Please choose source account");
                context.Call(new AccountSelectDialog(), ResumeAfterAccountSelectDialog);
            }
            else
            {
                await context.PostAsync(
                    "Sorry, not enough information provided.  \nPlease specify *target account* and amount of funds to transfer.");
                context.Done(false);
            }
        }

        private async Task ResumeAfterAccountSelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            if (context.ConversationData.TryGetValue(DataStrings.ActiveAccount, out _selectedAccount))
                await ValidateAccounts(context);
            else
                context.Fail(new Exception("Something went wrong"));
        }

        private async Task ValidateAccounts(IDialogContext context)
        {
            if (_selectedAccount.Id == _entityProps.Account.Id)
            {
                await context.PostAsync("Sorry, you can't transfer to the same account");
                context.Done(false);
            }
            else
            {
                PromptDialog.Confirm(context, ResumeAfterConfirmationPrompt,
                    $"OK, transfer **{_entityProps.MoneyAmount:C}** from **{_selectedAccount.Name}** to **{_entityProps.Account.Name}**?");
            }
        }

        private async Task ResumeAfterConfirmationPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            var proceed = await result;

            if (proceed)
            {
                if (_selectedAccount.Id == _entityProps.Account.Id)
                {
                    await context.PostAsync("Sorry, you can't transfer to the same account");
                    context.Done(false);
                }
                else
                {
                    AccountDataController.PerformInternalTransfer(_selectedAccount, _entityProps);

                    var message = context.MakeMessage();
                    var attachment = GetReceiptCard(_entityProps, _selectedAccount);
                    message.Attachments.Add(attachment);
                    await context.PostAsync(message);

                    context.Done(true);
                }
            }
            else
                context.Done(false);
        }
        
        //since currently global variables are used to store entities and active account, there is no need for parameters in this method
        //however, future refactoring is planned for the project.
        private static Attachment GetReceiptCard(EntityProps props, Account activeAccount)
        {
            var receiptCard = new ReceiptCard
            {
                Title = "Funds Transferred",
                Facts = new List<Fact> { new Fact("Transfer from", activeAccount.Name), new Fact("Transfer to", props.Account.Name) },
                Items = new List<ReceiptItem>
                {
                    new ReceiptItem("Amount", price: $"{props.MoneyAmount:C}", image: new CardImage(url: "https://d30y9cdsu7xlg0.cloudfront.net/png/3050-200.png")),
                },
                Tax = null,
                Total = $"{props.MoneyAmount:C}",
                Buttons = new List<CardAction>
                {
                    new CardAction(
                        ActionTypes.OpenUrl,
                        "View online",
                        "http://example.com/")
                }
            };

            return receiptCard.ToAttachment();
        }
    }
}