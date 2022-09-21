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
    <a href="https://www.youtube.com/watch?v=7rRr-QrRWckhttps">View Demo</a>
    ·
    <a href="https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/issues">Report Bug</a>
    ·
    <a href="https://docs.google.com/presentation/d/15G2gJ_l0Yf6BbbnAvDusuyo-6oca0zkx/edit?usp=sharing&ouid=110656000818841743215&rtpof=true&sd=true">View Presentation</a>
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
    <li><a href="#implementation-video">Implementation</a></li>
    <li><a href="#flow-chart">Flow Chart</a></li>
    <li><a href="#presentation">Presentation</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#further-reading">Further Reading</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

![App Screenshot](https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/blob/main/DEV%20Activity%20Chart.png)

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

The LUIS model for this example can be found under `CognitiveModels/FlightBooking.json` and the LUIS language model setup, training, and application configuration steps can be found [here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=cs).

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
    cd Dev-BankBot
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
  - Navigate to `Dev-BankBot` folder
  - Select `Dev-BankBot.csproj` file
  - Press `F5` to run the project

## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.5.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.
<p align="right">(<a href="readme-top">back to top</a>)</p>

## Presentation
[DEV BankingBot Presentation.pptx](https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/blob/main/Dev%20PPT%202.0.pptx)
<p align="right">(<a href="#readme-top">back to top</a>)</p>


## Flow Chart
![Flow Chart](https://github.com/mayank-cse/DEV-A-Virtual-Banking-Assistant/blob/main/Dev%20Flow%20Chart.png)



## Implementation Video

https://youtu.be/g6uBCjApQ0w

https://user-images.githubusercontent.com/72187020/191553184-fc2fbb80-280e-4992-8842-8cb9efc4ce3f.mp4


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
