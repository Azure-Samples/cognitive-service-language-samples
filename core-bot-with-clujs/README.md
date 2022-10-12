# CoreBot With CLU for JavaScript

Bot Framework v4 core bot sample using the CLU Recognizer.

This bot has been created using [Bot Framework](https://dev.botframework.com); it shows how to:

- Use [CLU][CLU_ServiceDocHomepage] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Prompt for and validate requests for information from the user

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses [Conversational Language Understanding (CLU)][CLU_ServiceDocHomepage], an AI based cognitive service, to implement language understanding. The service uses natively multilingual models, which means that users would be able to train their models in one language but predict in others. Users of the service have access to the [language studio][languagestudio], which simplifies the process of adding/importing data, labelling it, training a model, and then finally evaluating it. For more information, visit the official [service docs][CLU_ServiceDocHomepage].

### Install NodeJS

- [Node.js](https://nodejs.org) version 10.14.1 or higher

    ```bash
    # determine node version
    node --version
    ```

### Create a Conversational Language Understanding Application

The CLU model for this example can be found under `cognitiveModels/FlightBooking.json` and the CLU language model setup, training, and application configuration steps can be found [here](https://learn.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/tutorials/bot-framework).

Once you created the CLU model, update `.env` with your `CluProjectName`, `CluDeploymentName`, `CluAPIKey` and `CluAPIHostName`.

```text
CluProjectName="Your CLU project name"
CluDeploymentName="Your CLU deployment name"
CluAPIKey="Your CLU Subscription key here"
CluAPIHostName="Your CLU App region here (i.e: westus.api.cognitive.microsoft.com)"
```

# To run the bot

Navigate to the directory containing this readme file and perform the following:

- Install modules

    ```bash
    npm install
    ```
- Setup CLU

The prerequisite outlined above contain the steps necessary to provision a conversational language understanding model.

- Start the bot

    ```bash
    npm start
    ```
## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.9.0 or greater from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

## Deploy the bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.


## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Gathering Input Using Prompts](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Restify](https://www.npmjs.com/package/restify)
- [dotenv](https://www.npmjs.com/package/dotenv)

[CLU_ServiceDocHomepage]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/overview
[CLU_ServiceQuickStart]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/quickstart
[languagestudio]: https://language.azure.com/