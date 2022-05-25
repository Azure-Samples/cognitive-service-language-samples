// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Azure.AI.Language.Conversations;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Clu
{
    /// <summary>
    /// A helper class that creates and populate <see cref="RecognizerResult"/> from a <see cref="AnalyzeConversationResult"/> instance.
    /// </summary>
    internal static class RecognizerResultBuilder
    {
        private const string MetadataKey = "$instance";

        private static readonly HashSet<string> DateSubtypes = new HashSet<string>
        {
            "date",
            "daterange",
            "datetime",
            "datetimerange",
            "duration",
            "set",
            "time",
            "timerange"
        };

        private static readonly HashSet<string> GeographySubtypes = new HashSet<string>
        {
            "poi",
            "city",
            "countryRegion",
            "continent",
            "state"
        };

        public static RecognizerResult BuildRecognizerResultFromCluResponse(AnalyzeConversationResult cluResult, string utterance)
        {
            var recognizerResult = new RecognizerResult
            {
                Text = utterance,
                AlteredText = cluResult.Query
            };

            // CLU Projects can be Conversation projects (LuisVNext) or Orchestration projects that
            // can retrieve responses from other types of projects (Question answering, LUIS or Conversations)
            var projectKind = cluResult.Prediction.ProjectKind;

            if (projectKind == ProjectKind.Conversation)
            {
                UpdateRecognizerResultFromConversations((ConversationPrediction)cluResult.Prediction, recognizerResult);
            }
            else
            {
                // workflow projects can return results from LUIS, Conversations or QuestionAnswering Projects
                var orchestrationPrediction = (OrchestratorPrediction)cluResult.Prediction;

                // finding name of the target project, then finding the target project type
                var respondingProjectName = orchestrationPrediction.TopIntent;
                var targetIntentResult = orchestrationPrediction.Intents[respondingProjectName];

                // targetIntentResult.TargetKind is currently internal but will be changed in next version.
                // GetType() is used temporarily.

                // var targetKind = targetIntentResult.TargetKind;
                var targetKind = targetIntentResult.GetType().Name;

                switch (targetKind)
                {
                    case "ConversationTargetIntentResult":
                        var conversationTargetIntentResult = (ConversationTargetIntentResult)targetIntentResult;
                        UpdateRecognizerResultFromConversations(conversationTargetIntentResult.Result.Prediction, recognizerResult);
                        break;

                    case "LuisTargetIntentResult":
                        var luisTargetIntentResult = (LuisTargetIntentResult)targetIntentResult;
                        UpdateRecognizerResultFromLuis(luisTargetIntentResult, recognizerResult);
                        break;

                    case "QuestionAnsweringTargetIntentResult":
                        var questionAnsweringTargetIntentResult = (QuestionAnsweringTargetIntentResult)targetIntentResult;
                        UpdateRecognizerResultFromQuestionAnswering(questionAnsweringTargetIntentResult, recognizerResult);
                        break;
                }
            }

            AddProperties(cluResult, recognizerResult);

            return recognizerResult;
        }

        /// <summary>
        /// Returns a RecognizerResult from a conversations project response.
        /// 
        /// Intents: List of Intents with their confidence scores.
        /// Entities: has the object: { "entities" : [{entity1}, {entity2}] }
        /// Properties: Additional information returned by the service.
        /// 
        /// </summary>
        private static void UpdateRecognizerResultFromConversations(ConversationPrediction conversationPrediction, RecognizerResult recognizerResult)
        {
            recognizerResult.Intents = GetIntents(conversationPrediction);
            recognizerResult.Entities = ExtractEntitiesAndMetadata(conversationPrediction);
        }

        /// <summary>
        /// Returns a RecognizerResult from a question answering response received by an orchestration project.
        /// The recognizer result has similar format to the one returned by the QnAMaker Recognizer:
        /// 
        /// Intents: Indicates whether an answer has been found and contains the confidence score.
        /// Entities: has the object: { "answer" : ["topAnswer.answer"] }
        /// Properties: All answers returned by the Question Answering service.
        /// 
        /// </summary>
        private static void UpdateRecognizerResultFromQuestionAnswering(QuestionAnsweringTargetIntentResult qaResult, RecognizerResult recognizerResult)
        {
            var qnaAnswers = qaResult.Result.Answers;

            if (qnaAnswers.Count > 0)
            {
                var topAnswer = qnaAnswers[0];
                foreach (var answer in qnaAnswers)
                {
                    if (answer.ConfidenceScore > topAnswer.ConfidenceScore)
                    {
                        topAnswer = answer;
                    }
                }

                recognizerResult.Intents.Add(CluRecognizer.QuestionAnsweringMatchIntent, new IntentScore {Score = topAnswer.ConfidenceScore});

                var answerArray = new JArray {topAnswer.Answer};
                ObjectPath.SetPathValue(recognizerResult, "entities.answer", answerArray);

                recognizerResult.Properties["answers"] = qnaAnswers;
            }
            else
            {
                recognizerResult.Intents.Add("None", new IntentScore {Score = 1.0f});
            }
        }

        /// <summary>
        /// Returns a RecognizerResult from a luis response received by an orchestration project.
        /// The recognizer result has similar format to the one returned by the LUIS Recognizer:
        /// 
        /// Intents: Dictionary with (Intent, confidenceScores) pairs.
        /// Entities: The Luis entities Object, same as the one returned by the LUIS Recognizer.
        /// Properties: Sentiment result (if available) as well as the raw luis prediction result.
        /// </summary>
        private static void UpdateRecognizerResultFromLuis(LuisTargetIntentResult luisResult, RecognizerResult recognizerResult)
        {
            var luisPredictionObject = (JObject)JObject.Parse(luisResult.Result.ToString())["prediction"];
            recognizerResult.Intents = GetIntents(luisPredictionObject);
            recognizerResult.Entities = ExtractEntitiesAndMetadata(luisPredictionObject);

            AddProperties(luisPredictionObject, recognizerResult);
            recognizerResult.Properties.Add("luisResult", luisPredictionObject);
        }

        private static IDictionary<string, IntentScore> GetIntents(ConversationPrediction prediction)
        {
            var result = new Dictionary<string, IntentScore>();
            foreach (var intent in prediction.Intents)
            {
                result.Add(intent.Category, new IntentScore {Score = intent.ConfidenceScore});
            }

            return result;
        }

        private static JObject ExtractEntitiesAndMetadata(ConversationPrediction prediction)
        {
            var entities = prediction.Entities;
            var entityObject = JsonConvert.SerializeObject(entities);
            var jsonArray = JArray.Parse(entityObject);
            var returnedObject = new JObject {{"entities", jsonArray}};

            return returnedObject;
        }

        private static void AddProperties(AnalyzeConversationResult conversationResult, RecognizerResult result)
        {
            var topIntent = conversationResult.Prediction.TopIntent;
            var projectKind = conversationResult.Prediction.ProjectKind;
            var detectedLanguage = conversationResult.DetectedLanguage;

            result.Properties.Add("projectKind", projectKind.ToString());

            if (topIntent != null)
            {
                result.Properties.Add("topIntent", topIntent);
            }

            if (detectedLanguage != null)
            {
                result.Properties.Add("detectedLanguage", detectedLanguage);
            }

            if (projectKind == ProjectKind.Workflow)
            {
                var prediction = (OrchestratorPrediction)conversationResult.Prediction;
                var targetProject = prediction.Intents[prediction.TopIntent];

                // temporarily renamed until next release of CLU SDK
                // var targetProjectKind = targetProject.TargetKind
                var targetProjectKind = targetProject.GetType();
                result.Properties.Add("targetIntentKind", targetProjectKind);
            }
        }

        private static IDictionary<string, IntentScore> GetIntents(JObject luisResult)
        {
            var result = new Dictionary<string, IntentScore>();
            var intents = (JObject)luisResult["intents"];
            if (intents != null)
            {
                foreach (var intent in intents)
                {
                    result.Add(NormalizeIntent(intent.Key), new IntentScore {Score = intent.Value["score"]?.Value<double>() ?? 0.0});
                }
            }

            return result;
        }

        private static JObject ExtractEntitiesAndMetadata(JObject prediction)
        {
            var entities = JObject.FromObject(prediction["entities"]);
            return (JObject)MapProperties(entities, false);
        }

        private static void AddProperties(JObject luis, RecognizerResult result)
        {
            var sentiment = luis["sentiment"];
            if (luis["sentiment"] != null)
            {
                result.Properties.Add("sentiment", new JObject(
                    new JProperty("label", sentiment["label"]),
                    new JProperty("score", sentiment["score"])));
            }
        }

        private static string NormalizeIntent(string intent)
        {
            return intent.Replace('.', '_').Replace(' ', '_');
        }

        private static string NormalizeEntity(string entity)
        {
            // Type::Role -> Role
            var type = entity.Split(':').Last();
            return type.Replace('.', '_').Replace(' ', '_');
        }

        private static JToken MapProperties(JToken source, bool inInstance)
        {
            var result = source;
            if (source is JObject obj)
            {
                var nobj = new JObject();

                // Fix datetime by reverting to simple timex
                if (!inInstance && obj.TryGetValue("type", out var type) && type.Type == JTokenType.String && DateSubtypes.Contains(type.Value<string>()))
                {
                    var timexs = obj["values"];
                    var arr = new JArray();
                    if (timexs != null)
                    {
                        var unique = new HashSet<string>();
                        foreach (var elt in timexs)
                        {
                            unique.Add(elt["timex"]?.Value<string>());
                        }

                        foreach (var timex in unique)
                        {
                            arr.Add(timex);
                        }

                        nobj["timex"] = arr;
                    }

                    nobj["type"] = type;
                }
                else
                {
                    // Map or remove properties
                    foreach (var property in obj.Properties())
                    {
                        var name = NormalizeEntity(property.Name);
                        var isArray = property.Value.Type == JTokenType.Array;
                        var isString = property.Value.Type == JTokenType.String;
                        var isInt = property.Value.Type == JTokenType.Integer;
                        var val = MapProperties(property.Value, inInstance || property.Name == MetadataKey);
                        if (name == "datetime" && isArray)
                        {
                            nobj.Add("datetimeV1", val);
                        }
                        else if (name == "datetimeV2" && isArray)
                        {
                            nobj.Add("datetime", val);
                        }
                        else if (inInstance)
                        {
                            // Correct $instance issues
                            if (name == "length" && isInt)
                            {
                                nobj.Add("endIndex", property.Value.Value<int>() + property.Parent["startIndex"].Value<int>());
                            }
                            else if (!((isInt && name == "modelTypeId") ||
                                       (isString && name == "role")))
                            {
                                nobj.Add(name, val);
                            }
                        }
                        else
                        {
                            // Correct non-$instance values
                            if (name == "unit" && isString)
                            {
                                nobj.Add("units", val);
                            }
                            else
                            {
                                nobj.Add(name, val);
                            }
                        }
                    }
                }

                result = nobj;
            }
            else if (source is JArray arr)
            {
                var narr = new JArray();
                foreach (var elt in arr)
                {
                    // Check if element is geographyV2
                    var isGeographyV2 = string.Empty;
                    foreach (var props in elt.Children())
                    {
                        var tokenProp = props as JProperty;
                        if (tokenProp == null)
                        {
                            break;
                        }

                        if (tokenProp.Name.Contains("type") && GeographySubtypes.Contains(tokenProp.Value.ToString()))
                        {
                            isGeographyV2 = tokenProp.Value.ToString();
                            break;
                        }
                    }

                    if (!inInstance && !string.IsNullOrEmpty(isGeographyV2))
                    {
                        var geoEntity = new JObject();
                        foreach (var props in elt.Children())
                        {
                            var tokenProp = (JProperty)props;
                            if (tokenProp.Name.Contains("value"))
                            {
                                geoEntity.Add("location", tokenProp.Value);
                            }
                        }

                        geoEntity.Add("type", isGeographyV2);
                        narr.Add(geoEntity);
                    }
                    else
                    {
                        narr.Add(MapProperties(elt, inInstance));
                    }
                }

                result = narr;
            }

            return result;
        }
    }
}
