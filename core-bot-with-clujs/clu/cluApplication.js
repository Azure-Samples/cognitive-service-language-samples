// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

class CluApplication
{
        constructor(projectName, deploymentName, endpointKey, endpoint){
            this.projectName = projectName;
            this.deploymentName = deploymentName;
            this.endpointKey = endpointKey;
            this.endpoint = endpoint;

            if (isNullOrWhitespace(projectName))
            {
                throw new ArgumentNullException("projectName value is Null or whitespace. Please use a valid projectName.");
            }

            if (isNullOrWhitespace(deploymentName))
            {
                throw new ArgumentException("deploymentName value is Null or whitespace. Please use a valid deploymentName.");
            }

            if (isNullOrWhitespace(endpointKey))
            {
                throw new ArgumentException("endpointKey value is Null or whitespace. Please use a valid endpointKey.");
            }

            if (isNullOrWhitespace(endpoint))
            {
                throw new ArgumentException("Endpoint value is Null or whitespace. Please use a valid endpoint.");
            }

            if (!tryParse(endpointKey))
            {
                throw new ArgumentException("\"{endpointKey}\" is not a valid CLU subscription key.");
            }

            if (!isWellFormedUriString(endpoint))
            {
                throw new ArgumentException("\"{endpoint}\" is not a valid CLU endpoint.");
            }
        }

        isNullOrWhitespace( input ) {
            return !input || !input.trim();
        }

        tryParse( key ){
            var pattern = new RegExp('^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$', 'i');
            return pattern.test(key);
        }

        isWellFormedUriString( url ){
                var pattern = new RegExp('^(https?:\\/\\/)?'+ // protocol
                  '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|'+ // domain name
                  '((\\d{1,3}\\.){3}\\d{1,3}))'+ // OR ip (v4) address
                  '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*'+ // port and path
                  '(\\?[;&a-z\\d%_.~+=-]*)?'+ // query string
                  '(\\#[-a-z\\d_]*)?$','i'); // fragment locator
                return pattern.test(url);
        }
}

module.exports.CluApplication = CluApplication;