using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContosoData.Contollers;
using ContosoData.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace ContosoBot.Dialogs
{
    [Serializable]
    public class TransactionsQueryDialog : TypingReply, IDialog
    {
        private Account _selectedAccount;
        private readonly EntityProps _entityProps;
        private string _customMessage;

        public TransactionsQueryDialog(LuisResult luisResult = null)
        {
            _entityProps = new EntityAssigner().AssignEntities(luisResult);
        }

        public async Task StartAsync(IDialogContext context)
        {
            //check if user supplied query manually
            if (_entityProps.DateRange != null ||
                _entityProps.MoneyAmount != 0 ||
                !string.IsNullOrEmpty(_entityProps.Encyclopedia))
            {
                context.Call(new AccountSelectDialog(suggestions: true ,message: "Awesome, just one more step. Please choose account to query"), PerformTransactionQuery);
            }
            else
            {
                var suggestionMessage = GetSuggestions(context);
                suggestionMessage.Text = "Superb! Select one of the search crieria. See 'help' for future advanced search tips.";

                await context.PostAsync(suggestionMessage);
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            switch (message.Text.ToLower())
            {
                case "quit":
                    context.Fail(null);
                    break;
                case "help":
                    await context.PostAsync("Please select 'Show Help' button during the action selection step to see full help dialog.");
                    await context.PostAsync(GetSuggestions(context));
                    break;
                case "latest":
                    await ResumeAfterPrompt(context, result);
                    break;
                case "name":
                    var nameFormDialog = FormDialog.FromForm(this.BuildEncyclopediaForm, FormOptions.PromptInStart);
                    context.Call(nameFormDialog, ResumeAfterPrompt);
                    break;
                case "date":
                    var dateFormDialog = FormDialog.FromForm(this.BuildDateForm, FormOptions.PromptInStart);
                    context.Call(dateFormDialog, ResumeAfterPrompt);
                    break;
                case "amount":
                    var amountFormDialog = FormDialog.FromForm(this.BuildAmountForm, FormOptions.PromptInStart);
                    context.Call(amountFormDialog, ResumeAfterPrompt);
                    break;
                default:
                    var suggestionMessage = GetSuggestions(context);
                    suggestionMessage.Text = "Sorry, didn't get that";
                    await context.PostAsync(suggestionMessage);
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        // Account selection
        private Task ResumeAfterPrompt(IDialogContext context, IAwaitable<object> result)
        {
            context.Call(new AccountSelectDialog(suggestions: true ,message: _customMessage + " Now, choose account to search in (last step, I promise)"), PerformTransactionQuery);
            _customMessage = string.Empty;
            return Task.CompletedTask;
        }

        private async Task PerformTransactionQuery(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync(PostTypingReply(context));


            var selection    = await result;
            _selectedAccount = selection as Account ?? new Account();

            var queryResult = AccountDataController.GetTransactionsFromEntities(_selectedAccount, _entityProps);

            var reply              = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments      = new List<Attachment>();

            foreach (var transaction in queryResult)
            {
                reply.Attachments.Add(
                    new ThumbnailCard()
                        {
                            Title    = $"{transaction.Amount:C} {(transaction.TransactionType == "deposit" ? "Deposit" : "Payment")}",
                            Subtitle = $"{transaction.RecepientName}, {transaction.DateTime:d}",
                            Text     = transaction.Description
                        }
                        .ToAttachment());

                if (reply.Attachments.Count >= 20)
                    break;
            }

            reply.Text = reply.Attachments.Count >= 20 ? "Showing first 20 results" : $"{reply.Attachments.Count} transactions found";

            //TODO: Show only 5-10 transactions at once and display 'Show more' button.

            await context.PostAsync(reply);

            context.Done(true);
        }

        private static IMessageActivity GetSuggestions(IDialogContext context)
        {
            var suggestionMessage = context.MakeMessage();
            suggestionMessage.InputHint = InputHints.ExpectingInput;
            suggestionMessage.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction{ Title = "Latest", Type=ActionTypes.ImBack, Value="Latest" },
                    new CardAction{ Title = "By Name", Type=ActionTypes.ImBack, Value="Name" },
                    new CardAction{ Title = "By Date", Type=ActionTypes.ImBack, Value="Date" },
                    new CardAction{ Title = "By Amount", Type=ActionTypes.ImBack, Value="Amount" }
                }
            };
            return suggestionMessage;
        }


        private IForm<PropsQuery> BuildDateForm()
        {
            OnCompletionAsyncDelegate<PropsQuery> processForm = async (context, state) =>
            {
                var dateRange = new DateRange
                {
                    Type  = "date",
                    Start = state.Date
                };
                _entityProps.DateRange = dateRange;

                _customMessage = $"Neat! I'll look for transactions from {state.Date:d}.";
            };

            return new FormBuilder<PropsQuery>()
                .Field(nameof(PropsQuery.Date))
                .OnCompletion(processForm)
                .Build();
        }
        private IForm<PropsQuery> BuildAmountForm()
        {
            OnCompletionAsyncDelegate<PropsQuery> processForm = async (context, state) =>
            {
                _entityProps.MoneyAmount = state.Amount;
                _customMessage = $"Crikey! I'll look for transactions that are exactly {state.Amount:C}.";
            };

            return new FormBuilder<PropsQuery>()
                .Field(nameof(PropsQuery.Amount))
                .OnCompletion(processForm)
                .Build();
        }
        private IForm<PropsQuery> BuildEncyclopediaForm()
        {
            OnCompletionAsyncDelegate<PropsQuery> processForm = async (context, state) =>
            {
                _entityProps.Encyclopedia = state.Encyclopedia;
                _customMessage = $"Sweet as! I'll look for transactions related to {state.Encyclopedia}.";
            };

            return new FormBuilder<PropsQuery>()
                .Field(nameof(PropsQuery.Encyclopedia))
                .OnCompletion(processForm)
                .Build();
        }
    }

    class PropsQuery
    {
        [Prompt("Hallelujah! What date are we after?")]
        public DateTime Date { get; set; }
        [Prompt("Fantastic! What's the total amount I should look for?")]
        public float Amount { get; set; }
        [Prompt("Smashing! Who are we looking for?")]
        public string Encyclopedia { get; set; }
    }
}