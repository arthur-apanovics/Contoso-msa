using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    //TODO: This class has been hacked together. Need to improve logic

    public class TransferDialog : IDialog
    {
        private EntityProps _entityProps;
        private Account _sourceAccount;
        private Account _targetAccount;
        private float _amount;

        public TransferDialog(LuisResult luisResult)
        {
            _entityProps   = new EntityAssigner().AssignEntities(luisResult);
            _targetAccount = _entityProps.Account;
            _amount        = _entityProps.MoneyAmount;
        }

        public async Task StartAsync(IDialogContext context)
        {
            //all info supplied ?
            if (_entityProps.Account != null && _entityProps.MoneyAmount != 0 && _sourceAccount != null)
                await ValidateAndConfirm(context);

            else if (_targetAccount != null && _sourceAccount == null)
                context.Call(new AccountSelectDialog(message: "Almost there! Choose source account", suggestions: true), ResumeAfterSourceAccountSelectDialog);

            else
                context.Call(new AccountSelectDialog(message: "Cool! Let's start by selecting the source account", suggestions: true), ResumeAfterSourceAccountSelectDialog);
        }

        private async Task ResumeAfterSourceAccountSelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            _sourceAccount = await result as Account;

            if (_targetAccount == null)
                context.Call(new AccountSelectDialog(message: "Fantastic! Now the target account", suggestions: true), ResumeAfterTargetAccountSelectDialog);
            else if (_amount == 0)
                PromptForAmount(context);
            else
                await ValidateAndConfirm(context);
        }

        private async Task ResumeAfterTargetAccountSelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            _targetAccount = await result as Account;

            if (_amount == 0)
                PromptForAmount(context);
            else
                await ValidateAndConfirm(context);
        }

        private void PromptForAmount(IDialogContext context)
        {
            PromptDialog.Number(context, ResumeAfterAmountSelectDialog, "Alrighty! How much should I transfer??",
                min: 0.1f, max: float.MaxValue);
        }

        private async Task ResumeAfterAmountSelectDialog(IDialogContext context, IAwaitable<double> result)
        {
            _amount = (float) await result;

            await ValidateAndConfirm(context);
        }

        private async Task ValidateAndConfirm(IDialogContext context)
        {
            if (_sourceAccount.Id == _targetAccount.Id)
            {
                await context.PostAsync("OK, great, nothing done... easy as. Next time try to transfer from two different accounts");
                context.Done(false);
            }
            else
            {
                PromptDialog.Confirm(context, ResumeAfterConfirmationPrompt,
                    $"Easy peasy. Transfer {_amount:C} from {_sourceAccount.Name} to {_targetAccount.Name}?");
            }
        }

        private async Task ResumeAfterConfirmationPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            var proceed = await result;

            if (proceed)
            {
                UpdateEntityProps();

                AccountDataController.PerformInternalTransfer(_sourceAccount, _entityProps);

                var message    = context.MakeMessage();
                var attachment = GetReceiptCard();
                message.Attachments.Add(attachment);
                await context.PostAsync(message);

                context.Done(true);
            }
            else
                context.Done(false);
        }

        private void UpdateEntityProps()
        {
            _entityProps.Account     = _targetAccount;
            _entityProps.MoneyAmount = _amount;
        }

        //since currently global variables are used to store entities and active account, there is no need for parameters in this method
        //however, future refactoring is planned for the project.
        private Attachment GetReceiptCard()
        {
            var receiptCard = new ReceiptCard
            {
                Title = "Funds Transferred",
                Facts = new List<Fact> { new Fact("From", _sourceAccount.Name), new Fact("To", _targetAccount.Name) },
                //Items = new List<ReceiptItem>
                //{
                //    new ReceiptItem("Fund Transfer", price: $"{_amount:C}", , image: new CardImage(url: "https://d30y9cdsu7xlg0.cloudfront.net/png/3050-200.png")),
                //},
                Tax     = "You wish",
                Total   = $"{_amount:C}",
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