using AdaptiveCards;
using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class depositAccount : CancelAndHelpDialog
    {
        private readonly StateService _stateService;
        private readonly UserRepository _userRespository;
        protected readonly IConfiguration Configuration;
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        public depositAccount(StateService stateService, UserRepository userRepository, IConfiguration configuration) : base(nameof(depositAccount))
        {
            //_stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _userRespository = userRepository;
            Configuration = configuration;
            _stateService = stateService;


            var waterfallSteps = new WaterfallStep[]
            {
                
                LocationStepAsync,
                MessageStepAsync,
                //FinalStepAsync
                //MessageVerificationStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new TextPrompt(EmailDialogID, EmailValidation);
            //AddDialog(new NumberPrompt<int>(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }

        

        private async Task<DialogTurnResult> LocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            // Save the email to user profile

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("You can select from the options below or can type your questions in the space provided."), cancellationToken);
            List<string> operationList = new List<string> { "Savings Account", "Current Account", "Fixed Deposit", "Recurring Deposit", "Previous Menu" };
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
                MessageFactory.Text("Finished typing", "Finished typing"),
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

        private async Task<DialogTurnResult> MessageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if ("Savings Account".Equals(operation))
            {
                if (userProfile.attendance)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Don't worry Mr. {userProfile.Name}, Your attendance has already been marked. For any issues kindly contact the manager"), cancellationToken);
                    return await stepContext.NextAsync(null, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Hello {userProfile.Name}, Your location is not verified yet."), cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(depositAccount), null, cancellationToken);
                }
            }
            if ("Current Account".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(ScheduleTask), new User(), cancellationToken);
                //return await stepContext.BeginDialogAsync(nameof(ScheduleTask), new User());
            }
            if ("Fixed Deposit".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(AddMarketReport), new MarketDetails(), cancellationToken);
            }
            if ("Recurring Deposit".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(DisplayProduct), new ProductDetails(), cancellationToken);
            }
            if ("Previous Menu".Equals(operation))
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("The selected option not found."), cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }

        }
        
    }

}
