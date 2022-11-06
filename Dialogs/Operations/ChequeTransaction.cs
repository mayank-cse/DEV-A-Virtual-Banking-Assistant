using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
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
    public class ChequeTransaction : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        CosmosDBClient _cosmosDBClient;
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        StateService _stateService;

        private const string CogServiceSecret = "db071ae08bab4e57b35a0ef7d2a8c216";
        private const string Endpoint = "https://dev262.cognitiveservices.azure.com/";

        public ChequeTransaction(IConfiguration configuration, CosmosDBClient cosmosDBClient, StateService stateService) : base(nameof(ChequeTransaction))
        {
            Configuration = configuration;
            _cosmosDBClient = cosmosDBClient;
            _stateService = stateService;

            var waterfallSteps = new WaterfallStep[]
            {
                GetChequeStepAsync,
                InputChequeStepAsync
                
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


        private async Task<DialogTurnResult> GetChequeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("You have selected cheque transaction"), cancellationToken);
            /*
            var pmoptions = new PromptOptions();
            pmoptions.Prompt = MessageFactory.Text("Please upload the signed cheque");
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), pmoptions, cancellationToken);*/
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please upload the signed cheque. " + "\n" + 
                "For Example - 'https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/blob/main/1_analysis.jpeg?raw=true'"+""+ "or "+ "\n" + "'https://github.com/Azure-Samples/cognitive-services-sample-data-files/raw/master/ComputerVision/Images/faces.jpg'"),
                //Choices = ChoiceFactory.ToChoices(new List<string> { "Enter Product ID", "View Last Product ID" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> InputChequeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync($"{result}");

            if (result != null && result.Length > 0)
            {
               
                var credentials = new ApiKeyServiceClientCreds(CogServiceSecret);
                var client = new ComputerVisionClient(credentials)
                {
                    Endpoint = Endpoint
                };
                
                //Console.WriteLine("====== OBJECT DETECTION: 1 =======");
                string imageUrl = result;
                DetectResult analysis = await client.DetectObjectsAsync(imageUrl);
                int flag = 0;
                //Console.WriteLine("====== OBJECT DETECTION: 2 =======");
                foreach (var obj in analysis.Objects)
                {
                    flag = 1;
                    await stepContext.Context.SendActivityAsync($"{obj.ObjectProperty} with confidence score {obj.Confidence} as location {obj.Rectangle.X},{obj.Rectangle.X + obj.Rectangle.W},{obj.Rectangle.Y}{obj.Rectangle.Y + obj.Rectangle.H}");
                }
                if(flag == 1)
                {
                    await stepContext.Context.SendActivityAsync($"Please upload a valid signed cheque");

                }
                else
                {
                    await stepContext.Context.SendActivityAsync($"Date: {TCheque.Date} ");
                    await stepContext.Context.SendActivityAsync($"Payment Details Line one: {TCheque.PaymentDetail}");
                    await stepContext.Context.SendActivityAsync($"Payment Details Line two: {TCheque.PaymentDetail2}");
                    await stepContext.Context.SendActivityAsync($"Target Account Number: {TCheque.AccountNumber}");
                    await stepContext.Context.SendActivityAsync($"Amount: {TCheque.Amount}");

                }


            }
            else
            {
                await stepContext.Context.SendActivityAsync("Please upload a cheque!");
            }
            //TCheque userProfile = await _stateService.TChequeAccessor.GetAsync(stepContext.Context, () => new TCheque());
            //userProfile.ValueFinder = "Amount";
            
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
/**
private async Task<DialogTurnResult> GetChequeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("You have selected cheque transaction"), cancellationToken);
            /*
            var pmoptions = new PromptOptions();
            pmoptions.Prompt = MessageFactory.Text("Please upload the signed cheque");
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), pmoptions, cancellationToken);
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please upload the signed cheque"),
                //Choices = ChoiceFactory.ToChoices(new List<string> { "Enter Product ID", "View Last Product ID" }),
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> InputChequeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = (IList)stepContext.Result;
            
            if (result != null && result.Count > 0)
            {
                var attach = (Attachment)result[0];
                if (attach.ContentType == "image/jpeg")
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(attach.ContentUrl, attach.Name);
                    await stepContext.Context.SendActivityAsync("Thanks for upload");
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Please upload a cheque in png or jpeg format!");
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Please upload a cheque!");
            }
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            //userProfile.ValueFinder = "Amount";
           
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }**/