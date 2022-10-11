// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class CluValidator {
    validate(projectName, deploymentName, endpointKey, endpoint) {
        this.projectName = projectName;
        this.deploymentName = deploymentName;
        this.endpointKey = endpointKey;
        this.endpoint = endpoint;

        if (this.isNullOrWhitespace(projectName)) {
            throw new Error('projectName value is Null or whitespace. Please use a valid projectName.');
        }

        if (this.isNullOrWhitespace(deploymentName)) {
            throw new Error('deploymentName value is Null or whitespace. Please use a valid deploymentName.');
        }

        if (this.isNullOrWhitespace(endpointKey)) {
            throw new Error('endpointKey value is Null or whitespace. Please use a valid endpointKey.');
        }

        if (this.isNullOrWhitespace(endpoint)) {
            throw new Error('Endpoint value is Null or whitespace. Please use a valid endpoint.');
        }

        if (!this.tryParse(endpointKey)) {
            throw new Error('"{endpointKey}" is not a valid CLU subscription key.');
        }

        if (!this.isWellFormedUriString(endpoint)) {
            throw new Error('"{endpoint}" is not a valid CLU endpoint.');
        }
    }

    isNullOrWhitespace(input) {
        return !input || !input.trim();
    }

    tryParse(key) {
        var pattern = new RegExp('^[0-9a-f]{32}', 'i');
        return pattern.test(key);
    }

    isWellFormedUriString(url) {
        var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
                  '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
                  '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
                  '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
                  '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
                  '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
        return pattern.test(url);
    }
}

module.exports.CluValidator = CluValidator;
