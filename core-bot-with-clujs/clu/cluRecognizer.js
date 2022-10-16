// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ConversationAnalysisClient } = require('@azure/ai-language-conversations');
const { AzureKeyCredential } = require('@azure/core-auth');
const { CluValidator } = require('./cluValidator');

class CluRecognizer {
    constructor(options) {
        this.validate(options);
        this.conversationsClient = new ConversationAnalysisClient(options.endpoint, new AzureKeyCredential(options.endpointKey));
        this.options = options;
        this.CluTraceLabel = 'CLU Trace';
    }

    async recognizeAsync(turnContext) {
        var utterance = turnContext.activity.text;
        return await this.recognizeInternalAsync(utterance, turnContext);
    }

    async recognizeInternalAsync(utterance, turnContext) {
        var request =
        {
            analysisInput:
            {
                conversationItem:
                {
                    text: utterance,
                    id: '1',
                    participantId: '1'
                }
            },
            parameters:
            {
                projectName: this.options.projectName,
                deploymentName: this.options.deploymentName
            },
            kind: 'Conversation'
        };

        var cluResponse = await this.conversationsClient.analyzeConversation(request);

        var traceInfo = { response: cluResponse };

        await turnContext.sendTraceActivity('CLU Recognizer', traceInfo, this.CluTraceLabel);
        return cluResponse;
    }

    validate(options) {
        const { endpoint, endpointKey, projectName, deploymentName } = options;
        const validator = new CluValidator();
        validator.validate(projectName, deploymentName, endpointKey, endpoint);
    }
}

module.exports.CluRecognizer = CluRecognizer;
