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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
//using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
//using Azure.AI.FormRecognizer.DocumentAnalysis;
using Newtonsoft.Json;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    public class translateLanguage : CancelAndHelpDialog
    {
        private readonly StateService _stateService;
        private readonly UserRepository _userRespository;
        protected readonly IConfiguration Configuration;
        private const string CogServiceSecret = "db071ae08bab4e57b35a0ef7d2a8c216";
        private const string Endpoint = "https://dev262.cognitiveservices.azure.com/";
        //private readonly string CheckProductDialogID = "CheckProductDlg";
        private static readonly string key = "3d323a1a7ab4403ab0e74b0eacbad57a";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";

        // location, also known as region.
        // required if you're using a multi-service or regional (not global) resource. It can be found in the Azure portal on the Keys and Endpoint page.
        private static readonly string location = "eastus";
        public translateLanguage(StateService stateService, UserRepository userRepository, IConfiguration configuration) : base(nameof(translateLanguage))
        {
            //_stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _userRespository = userRepository;
            Configuration = configuration;
            _stateService = stateService;


            var waterfallSteps = new WaterfallStep[]
            {

                LanguageStepAsync,
                LangageDetectStepAsync
                //FinalStepAsync
                //MessageVerificationStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new TextPrompt(EmailDialogID, EmailValidation);
            //AddDialog(new NumberPrompt<int>(CheckProductDialogID, ProductExistsValidation));

            InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());


            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text("Please say something in language you want me to talk. Example 'j'aime le code et les sortes.'")

            }, cancellationToken);
        }
        private async Task<DialogTurnResult> LangageDetectStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Language"] = (string)stepContext.Result;
            string lang = (string)stepContext.Values["Language"];
            string route = "/translate?api-version=3.0&from=en&to=fr&to=zu";
            string textToTranslate = "Thank You for choosing Bank of Baroda";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(Endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                // location required if you're using a multi-service or regional (not global) resource.
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Translated to English : {result}"), cancellationToken);
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);

        }
    }
}
