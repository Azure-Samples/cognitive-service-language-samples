# CoreBot With CLU

Bot Framework v4 core bot sample using the CLU Recognizer.

This bot has been created using [Bot Framework](https://dev.botframework.com); it shows how to:

- Use [CLU][CLU_ServiceDocHomepage] to implement core AI capabilities
- Implement a multi-turn conversation using Dialogs
- Prompt for and validate requests for information from the user

## Prerequisites

This sample **requires** prerequisites in order to run.

### Overview

This bot uses [Conversational Language Understanding (CLU)][CLU_ServiceDocHomepage], an AI based cognitive service, to implement language understanding. The service uses natively multilingual models, which means that users would be able to train their models in one language but predict in others. Users of the service have access to the [language studio][languagestudio], which simplifies the process of adding/importing data, labelling it, training a model, and then finally evaluating it. For more information, visit the official [service docs][CLU_ServiceDocHomepage].


### Install .NET Core CLI

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 3.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

### Get started

Follow the [tutorial](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/tutorials/bot-framework) in the CLU documentation. 

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

[CLU_ServiceDocHomepage]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/overview
[CLU_ServiceQuickStart]: https://docs.microsoft.com/azure/cognitive-services/language-service/conversational-language-understanding/quickstart
[languagestudio]: https://language.azure.com/