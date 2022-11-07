// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.13.1

using AdaptiveCards;
using DevVirtualBankingAssistant.Dialogs.Operations;
using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.AI.QnA;
using Azure.AI.Language.QuestionAnswering;
using Azure;

using System.Configuration;

namespace DevVirtualBankingAssistant.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;
        UserRepository _userRepository;
        CDSRecognizer _luisRecognizer;
        private readonly string EmailVerificationCodeDialogID = "EmailVerificationCodeDlg";
        StateService _stateService;
        CosmosDBClient _cosmosDBClient;
        CosmoDBClientToDo _cosmoDBClientToDo;
        
        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog( ILogger<MainDialog> logger, IConfiguration configuration, UserRepository userRepository, StateService stateService, CosmosDBClient cosmosDBClient, CosmoDBClientToDo cosmoDBClientToDo)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;
            _userRepository = userRepository;
           _stateService = stateService;
            _cosmosDBClient = cosmosDBClient;
            _cosmoDBClientToDo = cosmoDBClientToDo;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new NumberPrompt<int>(EmailVerificationCodeDialogID, EmailVerificationCodeValidation));
            AddDialog(new EmailAuthenticationDialog(_stateService, userRepository, Configuration));
            AddDialog(new depositAccount(_stateService, userRepository, configuration));
            AddDialog(new DetectLanguage(_stateService, userRepository, configuration));
            AddDialog(new AddProductsDialog(Configuration, cosmosDBClient, _stateService));
            AddDialog(new DisplayProduct(Configuration, cosmosDBClient, _stateService));
            //AddDialog(new FindBank(Configuration, cosmosDBClient, _stateService));
            AddDialog(new ScheduleTask(_stateService,_cosmoDBClientToDo, Configuration));
            AddDialog(new UpdateProductDialog(_cosmosDBClient, _stateService));
            AddDialog(new Transaction(Configuration, cosmosDBClient, _stateService));
            AddDialog(new registerComplaint(Configuration, cosmosDBClient, _stateService));
            AddDialog(new ChequeTransaction(Configuration, cosmosDBClient, _stateService));
            AddDialog(new InvoiceDetection(Configuration, cosmosDBClient, _stateService));
            AddDialog(new RemoveProductsDialog(_cosmosDBClient, _stateService));
            AddDialog(new ViewAllProductsDialog(_cosmosDBClient, _stateService));
            AddDialog(new scheduleMeeting(_stateService, userRepository, configuration));
            AddDialog(new Remarks(Configuration, cosmosDBClient, _stateService));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                InitialStepAsync,
                NameStepAsync,
                AuthenticateStepAsync,
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private void AddDialog(scheduleMeeting scheduleMeeting, object _)
        {
            throw new NotImplementedException();
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());


            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Kindly share your details for me to recognize you. What is your name?")
                }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AuthenticateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                userProfile.Name = (string)stepContext.Result;

                await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }

            if (string.IsNullOrEmpty(userProfile.Email))
            {
                return await stepContext.BeginDialogAsync(nameof(EmailAuthenticationDialog), null, cancellationToken);
            }
            else
            {
                if (userProfile.UserAuthenticated)
                {
                    if(userProfile.Name == null)
                    {
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, cancellationToken);

                    }
                    
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Welcome back {userProfile.Name}, How can I help you today?"), cancellationToken);
                    return await stepContext.NextAsync(null, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello {userProfile.Name}, Your email is not verified."), cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(EmailAuthenticationDialog), null, cancellationToken);
                }

            }
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please wait while I authenticate with your Product Catalog..."), cancellationToken);

            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("How can I help you today?"), cancellationToken);
            List<string> operationList = new List<string> { "Deposit Accounts", "Transaction", "Detect Language", "Cheque Transaction", "Khata Book", "Invoice Analyser", "Service Product Record", "Register Complaint", "Locate ATM/Branch", "Schedule Meeting", "Hi! How are you", "Exit" };
            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = operationList.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };
            //Typing... Message
            await stepContext.Context.SendActivitiesAsync(
                new Activity[] {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value= 5000 },
                MessageFactory.Text("...", "..."),
                    },
                    cancellationToken);
            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(operationList),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if ("Deposit Accounts".Equals(operation))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello {userProfile.Name}, Your location is not verified yet."), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(depositAccount), null, cancellationToken);
            }
            else if ("Detect Language".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(DetectLanguage), new User(), cancellationToken);
            
            }
            else if("Locate ATM/Branch".Equals(operation))
            {
            
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Please visit the link for locating the ATM's - 'https://www.bankofbaroda.in/locate-us/atms."), cancellationToken);

                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else if ("Transaction".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(Transaction), new transactionDetails(), cancellationToken);
            }
            else if("Schedule Meeting".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(scheduleMeeting), new teams(), cancellationToken);
            }
            else if("Service Product Record".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(DisplayProduct), new Product(), cancellationToken);
            }
            else if("Cheque Transaction".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(ChequeTransaction), new transactionDetails(), cancellationToken);
            }
            else if("Khata Book".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(ScheduleTask), new User(), cancellationToken);
            }
            else if ("Invoice Analyser".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(InvoiceDetection), new InvoiceData(), cancellationToken);
            }
            else if ("Register Complaint".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(registerComplaint), new MarketDetails(), cancellationToken);
            }

            else if ("Exit".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(Remarks), new User(), cancellationToken);
                }
            else
            {
                //Getting answer from Azure Language Studio Question Answering
                return await GetAnswerFromQnAMaker(operation,stepContext, cancellationToken);
                
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
        private async Task<DialogTurnResult> GetAnswerFromQnAMaker(string question, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Uri endpoint = new Uri(Configuration["FAQENDPOINT"]);
            AzureKeyCredential credential = new AzureKeyCredential(Configuration["FAQKEY"]);
            string projectName = Configuration["FAQPROJECTNAME"];
            string deploymentName = Configuration["FAQDEPLOYMENTNAME"];
            //Get Answer from Azure Language Studio


            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);

            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            Response<AnswersResult> response = client.GetAnswers(question, project);
            foreach (KnowledgeBaseAnswer answer in response.Value.Answers)

            {
                 await stepContext.Context.SendActivityAsync(MessageFactory.Text(answer.Answer), cancellationToken);
            }

           return await stepContext.BeginDialogAsync(InitialDialogId, cancellationToken);

        }
       

        private async Task<bool> EmailVerificationCodeValidation(PromptValidatorContext<int> promptcontext, CancellationToken cancellationtoken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(promptcontext.Context, () => new UserProfile());
            int verificationCode = promptcontext.Recognized.Value;

            if (verificationCode == userProfile.OTP)
            {
                userProfile.UserAuthenticated = true;
                await _stateService.UserProfileAccessor.SetAsync(promptcontext.Context, userProfile);
                return true;
            }
            await promptcontext.Context.SendActivityAsync("The verification code you entered is incorrect. Please enter the correct code.", cancellationToken: cancellationtoken);
            return false;
        }
    }
}
