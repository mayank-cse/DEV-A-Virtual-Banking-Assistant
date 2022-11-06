using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class scheduleMeeting : CancelAndHelpDialog
    {
        private readonly StateService _stateService;
        private readonly UserRepository _userRespository;
        protected readonly IConfiguration Configuration;
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        public scheduleMeeting(StateService stateService, UserRepository userRepository, IConfiguration configuration) : base(nameof(scheduleMeeting))
        {
            //_stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _userRespository = userRepository;
            Configuration = configuration;
            _stateService = stateService;


            var waterfallSteps = new WaterfallStep[]
            {

                TimeStepAsync,
                ReasonStepAsync,
                teamsAsync
                //FinalStepAsync
                //MessageVerificationStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new TextPrompt(EmailDialogID, EmailValidation);
            //AddDialog(new NumberPrompt<int>(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> TimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            // Save the email to user profile

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("At what date and time should I schedule a meet with our officials. format - 'YYYY-MM-DD HH:MM:SS'"), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("At what date and time should I schedule a meet with our officials. format - 'YYYY-MM-DD HH:MM:SS'")
            }, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
        
            
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.Time = (string)stepContext.Result;
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("What is the Reason to schedule the meet? "), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("What is the Reason to schedule the meet?")
            }, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

        }
        private async Task<DialogTurnResult> teamsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            // Save the email to user profile
            userProfile.Reason = (string)stepContext.Result;


            await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Please wait while I schedule a teams meet for {userProfile.Time}."), cancellationToken);

            // trigger the power automate flow to send email
            bool status = await _userRespository.createTeamsMeet(userProfile.Name, userProfile.Email, userProfile.Time, userProfile.Reason, Configuration["PowerAutomateMeet"]);

            // verify the response status of the api call
            if (status)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank You! Teams meet have been created."), cancellationToken);

            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Teams meet could not be created please try again later after some time."), cancellationToken);
                //return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);

        }

    }
}
