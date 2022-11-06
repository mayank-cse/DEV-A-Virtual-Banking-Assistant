using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
//Using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace DevVirtualBankingAssistant.Dialogs.Operations
{
    class ApiKeyServiceClientCreds : ServiceClientCredentials
    {
        private readonly string subscriptionKey;
        public ApiKeyServiceClientCreds(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
        }
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
    class CognitiveService
    {


        private const string CogServiceSecret = "db071ae08bab4e57b35a0ef7d2a8c216";
        private const string Endpoint = "https://dev262.cognitiveservices.azure.com/";
        

        public static async Task DetectLanguage()
        {
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("The selected option not found."), cancellationToken);

            var credentials = new ApiKeyServiceClientCreds(CogServiceSecret);
            var client = new TextAnalyticsClient(credentials)
            {
                Endpoint = Endpoint
            };
            var inputData = new LanguageBatchInput(
                new List<LanguageInput>
                {
                    new LanguageInput("1","j'aime le code et les sortes."),
                    new LanguageInput("2","Oye, mi nombre es colby."),
                    new LanguageInput("3","Hi this is mayank trying to learn cognitive services"),


                });
            var results = await client.DetectLanguageBatchAsync(inputData);
            Console.WriteLine("====== LANGUAGE RECOGNITION =======");

            foreach (var document in results.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id}, Language : {document.DetectedLanguages[0].Name}");
            }
            Console.WriteLine("\n");
        }
        

        public static async Task DetectSentiment()
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
                    new MultiLanguageInput("1", "I hate it here", "en"),
                    new MultiLanguageInput("2", "what a great place", "en"),
                    new MultiLanguageInput("3", "I'm confused", "en")
                });
            var results = await client.SentimentBatchAsync(inputData);
            Console.WriteLine("====== SENTIMENT ANALYSIS =======");

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
                Console.WriteLine($"Document ID: {document.Id} is {SentimentMeaning}, Sentiment Score : {document.Score}");
            }
            Console.WriteLine("\n");
        }
        public static async Task DetectObject()
        {

            var credentials = new ApiKeyServiceClientCreds(CogServiceSecret);
            var SentimentMeaning = "";
            var client = new ComputerVisionClient(credentials)
            {
                Endpoint = Endpoint
            };
            Console.WriteLine("====== OBJECT DETECTION: 1 =======");
            string imageUrl = "https://github.com/Azure-Samples/cognitive-services-sample-data-files/raw/master/ComputerVision/Images/faces.jpg";
            DetectResult analysis = await client.DetectObjectsAsync(imageUrl);
            objectInfo(analysis);
            Console.WriteLine("====== OBJECT DETECTION: 2 =======");
            string imageUrl2 = "https://github.com/Azure-Samples/cognitive-services-dotnet-sdk-samples/raw/master/CustomVision/ObjectDetection/Images/scissors/scissors_12.jpg";
            DetectResult analysis2 = await client.DetectObjectsAsync(imageUrl2);
            objectInfo(analysis2);


        }
        public static void objectInfo(DetectResult analysis)
        {
            foreach (var obj in analysis.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} with confidence score {obj.Confidence} as location {obj.Rectangle.X},{obj.Rectangle.X + obj.Rectangle.W},{obj.Rectangle.Y}{obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine("\n");

        }
    }
}