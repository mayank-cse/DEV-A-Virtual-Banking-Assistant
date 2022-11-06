using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class Remarks : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        CosmosDBClient _cosmosDBClient;
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        StateService _stateService;
        private const string CogServiceSecret = "db071ae08bab4e57b35a0ef7d2a8c216";
        private const string Endpoint = "https://dev262.cognitiveservices.azure.com/";

        public Remarks(IConfiguration configuration, CosmosDBClient cosmosDBClient, StateService stateService) : base(nameof(Remarks))
        {
            Configuration = configuration;
            _cosmosDBClient = cosmosDBClient;
            _stateService = stateService;

            var waterfallSteps = new WaterfallStep[]
            {
                GetRemarkStepAsync,
                AnalyseRemarkStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));

            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AddMoreProductsDialog());
            AddDialog(new GetProductIDDialog(_cosmosDBClient));
            //AddDialog(new TextPrompt(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> GetRemarkStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank You for choosing bank of baroda.Before exiting please provide remarks for the experience with Dev"), cancellationToken);
            /*
            var pmoptions = new PromptOptions();
            pmoptions.Prompt = MessageFactory.Text("Please upload the signed cheque");
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), pmoptions, cancellationToken);*/
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Thank You for choosing bank of baroda.Before exiting please provide remarks for the experience with Dev"),
                //Choices = ChoiceFactory.ToChoices(new List<string> { "Enter Product ID", "View Last Product ID" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> AnalyseRemarkStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var remark = (string)stepContext.Result;

            if (remark != null && remark.Length > 0)
            {
                var credentials = new ApiKeyServiceClientCreds(CogServiceSecret);
                var SentimentMeaning = "";
                var client = new TextAnalyticsClient(credentials)
                {
                    Endpoint = Endpoint
                };

                var inputData = new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>
                    {
                        new MultiLanguageInput("1", remark, "en")
                    });
                var results = await client.SentimentBatchAsync(inputData);
                // Console.WriteLine("====== SENTIMENT ANALYSIS =======");

                foreach (var document in results.Documents)
                {
                    if (document.Score > 0.5)
                    {
                        SentimentMeaning = "Positive";
                    }
                    else
                    {
                        SentimentMeaning = "Negative";
                    }
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sentiment Analysed - {SentimentMeaning}, Sentiment Score : {document.Score}"), cancellationToken);
                }

                if (SentimentMeaning.Equals("Positive"))
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I am glad you liked me! I will try my best to perform even better next time. Thank You for choosing Bank of Baroda"), cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I am sorry for my bad performance. I will try my best to learn and be better next time. Thank You for choosing Bank of Baroda"), cancellationToken);

                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Please give a valid remark");
            }
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank You Mr. {userProfile.Name}. You have successfully logged out."), cancellationToken);
            
            userProfile.Email = null;
            userProfile.Name = null;
            userProfile.attendance = false;
            return await stepContext.CancelAllDialogsAsync();
            //return await stepContext.ReplaceDialogAsync(InitialDialogId, cancellationToken);

        }
    }
}
