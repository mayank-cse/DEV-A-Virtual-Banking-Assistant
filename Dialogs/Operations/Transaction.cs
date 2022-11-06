using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevVirtualBankingAssistant.Cards;
using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class Transaction : CancelAndHelpDialog
    {
        protected readonly IConfiguration Configuration;
        CosmosDBClient _cosmosDBClient;
        private readonly string CheckProductDialogID = "CheckProductDlg";
        StateService _stateService;

        public Transaction(IConfiguration configuration, CosmosDBClient cosmosDBClient, StateService stateService) : base(nameof(Transaction))
        {
            Configuration = configuration;
            _cosmosDBClient = cosmosDBClient;
            _stateService = stateService;

            var waterfallSteps = new WaterfallStep[]
            {
                GetcustomerIDStepAsync,
                GetAmountStepAsync,
                ToNameStepAsync,
                toUpiIDStepAsync,
                //timeStepAsync,
                RemarksStepAsync,
                //ProductCategoryStepAsync,
                MoreProductsStepAsync,
                MoreProductActStepAsync,
                SummaryStepAsync,
            };
            /*
             * "id": "cust1",
    "amount": "101",
    "Email": "mayank.guptacse1@gmail.com",
    "Name": "Mayank Gupta",
    "ToName": "100Mayank",
    "toUpiID": "abc@123",
    "time": "2022-10-11 11:11:11",
    "Remarks": "Payment for testing",
             */
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AddMoreProductsDialog());
            AddDialog(new GetProductIDDialog(_cosmosDBClient));
            AddDialog(new TextPrompt(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> GetcustomerIDStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("What is your PIN?"),
                //Choices = ChoiceFactory.ToChoices(new List<string> { "Enter Product ID", "View Last Product ID" }),
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> GetAmountStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["PIN"] = (string)stepContext.Result; ;
            string getProductID = (string)stepContext.Values["PIN"];

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("What is the amount you want to transfer?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ToNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.ValueFinder = "Amount";
            await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            stepContext.Values["Amount"] = (string)stepContext.Result;
            //stepContext.Values["RewardPoint"] = (int)(stepContext.Values["Amount"] / 100);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("What is the name of the person you want to transfer the amount to?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> toUpiIDStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["PersonName"] = (string)stepContext.Result;

            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.ValueFinder = "PersonName";
            await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);


            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Please share the upi id of {(string)stepContext.Values["PersonName"]}")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> RemarksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["UPIID"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Please share some remarks for the payment for me to remind you incase asked")
            }, cancellationToken);
        }
        
       
        private async Task<DialogTurnResult> MoreProductsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Remark"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to do more transactions?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> MoreProductActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var transactionDetails = (transactionDetails)stepContext.Options;
            var rewardPoint = (string)stepContext.Values["Amount"];
            if ((bool)stepContext.Result)
            {
                //return await stepContext.BeginDialogAsync(nameof(AddMoreProductsDialog), productDetails, cancellationToken);
                return await stepContext.NextAsync(transactionDetails, cancellationToken);

            }
            else
            {
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text("The transaction to '" + transactionDetails.transactionList[i].Name + "' already exists"), cancellationToken);

                return await stepContext.NextAsync(transactionDetails, cancellationToken);
            }
        }
        
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"scsdfffvfd{(string)stepContext.Values["Remark"]}"), cancellationToken);

            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            var transactionDetails = (transactionDetails)stepContext.Result;
            Amount amount = new Amount()
            {
                Id = "Cust2",
                Email = userProfile.Email,
                Name = userProfile.Name,
                time = "2022-11-11 11:11:11",
                //PIN = (string)stepContext.Values["PIN"],
                amount = (string)stepContext.Values["Amount"],
                toName = (string)stepContext.Values["PersonName"],
                toUpiID = (string)stepContext.Values["UPIID"],
                Remarks = (string)stepContext.Values["Remark"],
                rewardPoint = (string)stepContext.Values["Amount"]
            };
            transactionDetails.transactionList.Add(amount);
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Transaction Initiated."), cancellationToken);
            await _cosmosDBClient.CreateDBConnection(Configuration["CosmosEndPointURI"], Configuration["CosmosPrimaryKey"], Configuration["CosmosDatabaseIdtrans"], Configuration["CosmosContainerIdtrans"], Configuration["CosmosPartitionKeytrans"]);

            for (int i = 0; i < transactionDetails.transactionList.Count; i++)
            {
                bool flag = true;

                if (await _cosmosDBClient.AddItemsToContainerAsync(transactionDetails.transactionList[i].Id, transactionDetails.transactionList[i].amount, transactionDetails.transactionList[i].Email, transactionDetails.transactionList[i].Name, transactionDetails.transactionList[i].toName, transactionDetails.transactionList[i].toUpiID, transactionDetails.transactionList[i].time, transactionDetails.transactionList[i].Remarks, transactionDetails.transactionList[i].rewardPoint) == -1)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("The transaction to '" + transactionDetails.transactionList[i].Name + "' already exists"), cancellationToken);
                    flag = false;
                }

                if (flag)
                {
                    reply.Attachments.Add(Cards.Cards.GetHeroCard(transactionDetails.transactionList[i].toName, transactionDetails.transactionList[i].toUpiID, transactionDetails.transactionList[i].amount, transactionDetails.transactionList[i].Remarks).ToAttachment());
                }

            }

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Congratulations you have earned {(string)stepContext.Values["Amount"]} point for completing the transanction operation. Thank you for choosing Bank of Baroda."), cancellationToken);

            return await stepContext.EndDialogAsync(transactionDetails, cancellationToken);
        }

        private async Task<bool> ProductExistsValidation(PromptValidatorContext<string> promptcontext, CancellationToken cancellationtoken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(promptcontext.Context, () => new UserProfile());

            string product = promptcontext.Recognized.Value;

            if (await _cosmosDBClient.CheckProductAsync(product, userProfile.ValueFinder))
            {
                await promptcontext.Context.SendActivityAsync($"The {userProfile.ValueFinder} {product} already exists. Please give different {userProfile.ValueFinder}", cancellationToken: cancellationtoken);
                return false;
            }

            return true;
        }
    }
}
