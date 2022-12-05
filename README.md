<!-- #Dev - Bank Bot -->
<a name="readme-top"></a>
<h1 align="center">Dev - A Virtual Banking Assistant</h1>
  <p align="center">
    Today’s customers are extremely demanding, expecting fast, inspiring and relevant banking experiences in every moment of the banking journey. Virtual Assistance is the heart for bankers succeeding in delivering relevant customer experience, which is a continuous perpetual challenge, highly correlated to optimizing conversion, sales and increased revenue.
 <br>
 The virtual financial assistant Dev is designed to help customers more easily manage their money. Within the interactive interface, Dev converse to answer FAQs, provide reward and account balances, spending summaries, refund confirmations and credit scores. He can also identify duplicate charges and send bill reminders.
    <br />
    <a href="https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="#demo-video">View Demo</a>
    ·
    <a href="https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/issues">Report Bug</a>
    ·
    <a href="https://1drv.ms/p/s!AqPutyaMMDPchxk-rgpzvvOp-avH?e=39UbKT">View Presentation</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#prerequisites">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#overview">Overview</a></li>
        <li><a href="#install-net-core-cli">Installation</a></li>
        <li><a href="#create-a-luis-application-to-enable-language-understanding">Enable LUIS</a></li>
      </ul>
    </li>
    <li><a href="#to-try-this-sample">Try This Sample</a></li>
    <li><a href="#testing-the-bot-using-bot-framework-emulator">Emulator Testing</a></li>
    <li><a href="#deploy-the-bot-to-azure">Deploying</a></li>
    <li><a href="#flow-chart">Flow Chart</a></li>
    <li><a href="#presentation">Presentation</a></li>
    <li><a href="#implementation-video">Implementation</a></li>
    <li><a href="#demo-video">Demo Video</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#further-reading">Further Reading</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

<img width="960" alt="DEV Activity Chart" src="https://user-images.githubusercontent.com/72187020/200659507-6ab4b64f-197b-44e2-9d62-be26c4b8b101.png">


The virtual financial assistant Dev is designed to help customers more easily manage their money. Within the interactive interface, Dev provides reward and account balances, spending summaries, refund confirmations and credit scores. He can also identify duplicate charges and send bill reminders. 

Dev is designed for the customers to :
* Help customers more easily manage their money.
* Check on a loan status.
* Facilitating payments with mail alert. 
* Getting instant answers to FAQs.

Key Features of the Product :
* Instant Response with automatic time-to-time pop-ups (alerts).
* Proactively reaches out if a bill is higher than normal.
* Suspects fraud and sends mail alert.
* Provides reward and account balances, spending summaries, refund confirmations and credit scores. 
<!-- * Automating Business Operations for visible efficiency gains due to fast communication. -->

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Built With
Bot Framework v4 core bot sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to:

- Use [LUIS](https://www.luis.ai) to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Handle user interruptions for such things as `Help` or `Cancel`
- Prompt for and validate requests for information from the user

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses [LUIS](https://www.luis.ai), an AI based cognitive service, to implement language understanding.

### Install .NET Core CLI

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

- If you don't have an Azure subscription, create a [free account](https://azure.microsoft.com/free/).
- Install the latest version of the [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) tool. Version 2.0.54 or higher.

### Create a LUIS Application to enable language understanding

The LUIS model for this example can be found under `CognitiveModels/BankLuisModel.json` and the LUIS language model setup, training, and application configuration steps can be found [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=cs).

Once you created the LUIS model, update `appsettings.json` with your `LuisAppId`, `LuisAPIKey` and `LuisAPIHostName`.

```json
  "LuisAppId": "Your LUIS App Id",
  "LuisAPIKey": "Your LUIS Subscription key here",
  "LuisAPIHostName": "Your LUIS App region here (i.e: westus.api.cognitive.microsoft.com)"
```
<p align="right">(<a href="#readme-top">back to top</a>)</p>

## To try this sample

- In a terminal, navigate to `Dev-BankBot`

    ```bash
    # change into project folder
    cd DevVirtualBankingAssistant
    ```

- Run the bot from a terminal or from Visual Studio, choose option A or B.

  A) From a terminal

  ```bash
  # run the bot
  dotnet run
  ```

  B) Or from Visual Studio

  - Launch Visual Studio
  - File -> Open -> Project/Solution
  - Navigate to `DevVirtualBankingAssistant` folder
  - Select `DevVirtualBankingAssistant.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.5.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## The LUIS Bank Transaction Concept
The bot is built around a very typical banking scenario which has two main capabilities:
* Check balance
* Make a transfer

### This sample demonstrates:
* __Luis Intent detection__ 
	* Using `LuisRecognizer` not `LuisRecognizerMiddleware` 
	* Using Luis in middleware means every single message will go via Luis which is not necessary and costly in this scenario because once we have the intent and initial entities we no longer require Luis
* __Luis entity extraction__; getting the entities we have from the initial Luis utterance
* __Entity completion__; using bot dialogs to complete entities that were missing from initial Luis utterance
* __Basic bot dialogs with waterfall steps__


### Check Balance
Simple intent that displays a made-up balance for the user's account

To invoke the Check Balance feature
* __"Check my balance"__; no entities just the `Balance` intent

### Using Make a Transfer
To make a transfer, the user must provide four different entities. These can be included in the initial utterance; if they are not, the bot will use a dialog to complete them:
* __AccountLabel__; a [simple Luis entity](https://docs.microsoft.com/en-gb/azure/cognitive-services/LUIS/luis-concept-entity-types) to represent the nick name for the account to transfer from i.e. 'Joint', 'Savings', 'Current' or 'Sole' 
* __Money__; a [pre-built Luis Currency entity](https://docs.microsoft.com/en-gb/azure/cognitive-services/LUIS/luis-reference-prebuilt-currency) to represent the amount to be transferred
* __Date__; a [pre-built Luis DatetimeV2 entity](https://docs.microsoft.com/en-gb/azure/cognitive-services/LUIS/luis-reference-prebuilt-datetimev2) to represent the date the transfer should take place
* __Payee__; a [simple Luis entity](https://docs.microsoft.com/en-gb/azure/cognitive-services/LUIS/luis-concept-entity-types) to represent the label for the payment recipient. This will typically be a name or company name (The Luis model has very limited training here, so only 'Martin Kearn', 'Amy Griffiths', 'John Jones' and 'BT' are likely to work as a payee)

The Make a Transfer feature can be invoked using natural language including some, all or none or the required entities. Here are some examples:
* __"I want to make a transfer"__; the `Transfer` intent without any entities.
* __"Transfer from the joint account"__; the `Transfer` intent with the `AccountLabel` entity.
* __"Transfer £20 from the joint account"__; the `Transfer` intent with the `AccountLabel` and `Money` entities.
* __"Transfer £20 from the joint account on saturday"__; the `Transfer` intent with the `AccountLabel`, `Money` and `Date` entities.
* __"Transfer £20 from the joint account to martin kearn on saturday"__; the `Transfer` intent with the `AccountLabel`, `Money`, `Date` and `Payee` entities.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.
<p align="right">(<a href="readme-top">back to top</a>)</p>

## Presentation
[DEV BankingBot Presentation.pptx](https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/blob/main/Resources/Dev%20PPT%202.0.pptx)
<p align="right">(<a href="#readme-top">back to top</a>)</p>


## Flow Chart
<!--<img width="960" alt="Dev Flow Chart" src="https://user-images.githubusercontent.com/72187020/200659707-d40df1b0-0108-4bb5-a7c3-4510f6e2880a.png">-->
<img width="960" alt="Dev_FlowChart" src="https://user-images.githubusercontent.com/72187020/205633435-9d07b9dd-55a7-464f-b422-b94f70561f12.png">


## Implementation Video

https://youtu.be/g6uBCjApQ0w

https://user-images.githubusercontent.com/72187020/191553184-fc2fbb80-280e-4992-8842-8cb9efc4ce3f.mp4


## Demo Video

https://youtu.be/uHREvKiU0g8


https://user-images.githubusercontent.com/72187020/200647986-3a5145ac-0a07-470f-aa89-69735488d8de.mp4
<!-- TIME STAMP -->
<details>
  <summary>Time Stamp of Bot Feature Demo</summary>
  <ol>
    <li>1:29 - Demo Starts</li>
    <li>1:41 - OTP Verification</li>
    <li>2:21 - QnA Maker/ Language Studio Question Answering</li>
    <li>2:36 - Transaction (Microsoft Cosmo DB)</li>
    <li>3:57 - Detect Language (Azure Language Detection + Language Translation)</li>
    <li>4:50 - Cheque Transaction (Azure Computer Vision + Azure Object Detection)</li>
    <li>6:15 - Khata Book (Azure Cosmo DB)</li>
    <li>7:09 - Invoice Analyzer (Azure Form Recognizer)</li>
    <li>8:05 - Service Products (Azure Cosmo DB)</li>
    <li>(9:11) Cancel | Quit Intent</li>
    <li>11:29 - Register Complaint</li>
    <li>12:47 - Schedule Meeting (Microsoft Power Automate + Microsoft Teams)</li>
    <li>14:15 - Exit (Azure Sentiment Analysis)</li>
    <li>15:11 - Glance of the backend running code</li>
    <li>15:32 - Hosted (Azure Bot Service)</li>
    <li>16:04 - LUIS for transaction n checking balance</li>
  </ol>
</details>






<!-- CONTACT -->
## Contact

Mayank Gupta - [@MayankGuptaCse1](https://twitter.com/MayankGuptacse1) - mayank.guptacse1@gmail.com

Project Link: [https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant](https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant)

<p align="right">(<a href="#readme-top">back to top</a>)</p>


## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Gathering Input Using Prompts](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=csharp)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
