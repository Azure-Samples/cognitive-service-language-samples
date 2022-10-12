// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CluRecognizer } = require('../clu/cluRecognizer');

class FlightBookingRecognizer {
    constructor(config) {
        const cluIsConfigured = config && config.endpointKey && config.endpoint && config.projectName && config.deploymentName;
        if (cluIsConfigured) {
            this.recognizer = new CluRecognizer(config);
        }
    }

    get isConfigured() {
        return (this.recognizer !== undefined);
    }

    /**
     * Returns an object with preformatted CLU results for the bot's dialogs to consume.
     * @param {TurnContext} context
     */
    async executeCluQuery(context) {
        return await this.recognizer.recognizeAsync(context);
    }

    getFromEntities(response) {
        var result = response.result.prediction;
        let fromValue;

        for (const entity of result.entities) {
            if (entity.category === 'fromCity') {
                fromValue = entity.text;
            }
        }

        const fromAirportValue = fromValue;

        return { from: fromValue, airport: fromAirportValue };
    }

    getToEntities(response) {
        var result = response.result.prediction;
        let toValue;

        for (const entity of result.entities) {
            if (entity.category === 'toCity') {
                toValue = entity.text;
            }
        }

        const toAirportValue = toValue;

        return { to: toValue, airport: toAirportValue };
    }

    /**
     * This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
     * TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
     */
    getTravelDate(response) {
        const result = response.result.prediction;
        let datetimeEntity;

        for (const entity of result.entities) {
            if (entity.category === 'flightDate') {
                datetimeEntity = entity.resolutions;
            }
        }

        if (!datetimeEntity || !datetimeEntity[0]) return undefined;

        const timex = datetimeEntity[0].timex;
        if (!timex) return undefined;

        return timex;
    }

    topIntent(response) {
        return response.result.prediction.topIntent;
    }
}

module.exports.FlightBookingRecognizer = FlightBookingRecognizer;
