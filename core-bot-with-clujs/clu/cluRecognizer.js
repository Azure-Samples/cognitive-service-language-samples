// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ConversationAnalysisClient } = require("@azure/ai-language-conversations");
const { AzureKeyCredential } = require("@azure/core-auth");

class CluRecognizer
{            
    constructor(options)
    {
        this.conversationsClient = new ConversationAnalysisClient(options.endpoint, new AzureKeyCredential(options.endpointKey));
        this.options = options;
        this.CluTraceLabel = "CLU Trace";
    }

    async recognizeAsync(turnContext)
    {
        var utterance = turnContext.activity.text;
        console.log("Utterance is: " + utterance);
        var request =
        {
            analysisInput:
            {
                conversationItem:
                {
                    text: utterance,
                    id: "1",
                    participantId: "1",
                }
            },
            parameters:
            {
                projectName: this.options.projectName,
                deploymentName: this.options.deploymentName
            },
            kind: "Conversation"
        };

        var cluResponse = await this.conversationsClient.analyzeConversation(request);

        var traceInfo = { response: cluResponse };

        await turnContext.sendTraceActivity("CLU Recognizer", traceInfo, this.CluTraceLabel);
        return cluResponse;
    }

    async recognizeInternalAsync(utterance, turnContext)
    {
        console.log("Utterance received");
        var request =
        {
            analysisInput:
            {
                conversationItem:
                {
                    text: utterance,
                    id: "1",
                    participantId: "1",
                }
            },
            parameters:
            {
                projectName: _options.CluApplication.ProjectName,
                deploymentName: _options.CluApplication.DeploymentName,

                // Use Utf16CodeUnit for strings in .NET.
                stringIndexType: "Utf16CodeUnit",
            },
            kind: "Conversation",
        };

        var cluResponse = await _conversationsClient.analyzeConversation(request);

        var traceInfo = { response: cluResponse };

        await turnContext.sendTraceActivity("CLU Recognizer", traceInfo, CluTraceLabel);
        return cluResponse;
    }
}

module.exports.CluRecognizer = CluRecognizer;