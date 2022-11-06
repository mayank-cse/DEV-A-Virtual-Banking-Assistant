using DevVirtualBankingAssistant.Models;
using DevVirtualBankingAssistant.Services;
using DevVirtualBankingAssistant.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Azure.AI.Language.QuestionAnswering;
using Azure;
using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace DevVirtualBankingAssistant.Dialogs.Operations
{

    public class answerFAQ : ActivityHandler
    {
        public object EchoBotQnA { get; private set; }
        private IConfiguration _configuration;
        private readonly StateService _stateService;
        private readonly UserRepository _userRespository;

        public answerFAQ(StateService stateService, UserRepository userRepository, IConfiguration configuration)
        {
            //_stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _userRespository = userRepository;
            _configuration = configuration;
            _stateService = stateService;

        }



        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            await GetAnswerFromQnAMaker(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
        private async Task GetAnswerFromQnAMaker(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Uri endpoint = new Uri($"{_configuration["ENDPOINT"]}");
            AzureKeyCredential credential = new AzureKeyCredential($"{_configuration["KEY"]}");
            string projectName = $"{_configuration["PROJECTNAME"]}";
            string deploymentName = $"{_configuration["DEPLOYMENTNAME"]}";
            string question = turnContext.Activity.Text;


            QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);

            QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);

            Response<AnswersResult> response = client.GetAnswers(question, project);

            foreach (KnowledgeBaseAnswer answer in response.Value.Answers)

            {
                await turnContext.SendActivityAsync(MessageFactory.Text(answer.Answer), cancellationToken);
            }

            /*else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in QnA system. Please rephrase your question."), cancellationToken);

            }*/


        }
        

    }
}
