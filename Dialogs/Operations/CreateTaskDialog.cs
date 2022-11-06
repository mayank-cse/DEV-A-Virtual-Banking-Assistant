using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevVirtualBankingAssistant.Utilities;
using DevVirtualBankingAssistant.Dialogs.Operations;
using DevVirtualBankingAssistant.Models;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class CreateTaskDialog : ComponentDialog
    {
        
        private readonly CosmoDBClientToDo _cosmosDBClient;
        public CreateTaskDialog(CosmoDBClientToDo cosmosDBClient) : base(nameof(CreateTaskDialog))
        {
            
            _cosmosDBClient = cosmosDBClient;
            var waterfallSteps = new WaterfallStep[]
            {
                TasksStepAsync,
                ActStepAsync,
                MoreTasksStepAsync,
                SummaryStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new CreateMoreTaskDialog());

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TasksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please give the Payment record to add.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            stepContext.Values["Task"] = (string)stepContext.Result;
            userDetails.TasksList.Add((string)stepContext.Values["Task"]);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to Add more payment record?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> MoreTasksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            if ((bool)stepContext.Result)
            {
                return await stepContext.BeginDialogAsync(nameof(CreateMoreTaskDialog), userDetails, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userDetails, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Result;
            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Here are the payment record you provided - "), cancellationToken);
            for (int i = 0; i < userDetails.TasksList.Count; i++)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(userDetails.TasksList[i]), cancellationToken);
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please wait while I add your payment record to the database..."), cancellationToken);
            for (int i = 0; i < userDetails.TasksList.Count; i++)
            {
                if (await _cosmosDBClient.AddItemsToContainerAsync(User.UserID, userDetails.TasksList[i]) == -1)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("The payment record '" + userDetails.TasksList[i] + "' already exists"), cancellationToken);
                    
                }
                
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Add payment record operation completed. Thank you."), cancellationToken);

            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }
    }
}
