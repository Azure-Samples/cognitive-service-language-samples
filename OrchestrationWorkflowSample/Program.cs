using System;
using System.Linq;
using System.Text.Json;
using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;

namespace OrchestrationWorkflowSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Provide credentials
            Uri endpoint = new Uri("");
            AzureKeyCredential credential = new AzureKeyCredential("");

            ConversationAnalysisClient client = new ConversationAnalysisClient(endpoint, credential);

            //Provide project information
            string projectName = "";
            string deploymentName = "";

            Console.WriteLine("Input a query to your orchestration project:");
            
            string query = Console.ReadLine();

            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        text = query,
                        id = "1",
                        participantId = "1",
                    }
                },
                parameters = new
                {
                    projectName,
                    deploymentName,

                    // Use Utf16CodeUnit for strings in .NET.
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };

            Response response = client.AnalyzeConversation(RequestContent.Create(data));

            using JsonDocument result = JsonDocument.Parse(response.ContentStream);

            JsonElement conversationalTaskResult = result.RootElement;
            JsonElement orchestrationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");

            string topIntent = orchestrationPrediction.GetProperty("topIntent").GetString();

            Console.WriteLine($"The top intent was {topIntent}\n");
            Console.WriteLine($"The result from the connected project is as follows:\n");

            JsonElement targetIntentResult = orchestrationPrediction.GetProperty("intents").GetProperty(topIntent);

            //Conversational language understanding response
            if (targetIntentResult.GetProperty("targetProjectKind").GetString() == "Conversation")
            {

                JsonElement conversationPrediction = targetIntentResult.GetProperty("result").GetProperty("prediction");

                Console.WriteLine($"\tTop Intent: {conversationPrediction.GetProperty("topIntent").GetString()}");
                Console.WriteLine($"\tIntents:");
                foreach (JsonElement intent in conversationPrediction.GetProperty("intents").EnumerateArray())
                {
                    Console.WriteLine($"\t\tCategory: {intent.GetProperty("category").GetString()}");
                    Console.WriteLine($"\t\tConfidence: {intent.GetProperty("confidenceScore").GetSingle()}");
                    Console.WriteLine();
                }

                Console.WriteLine($"\tEntities:");
                foreach (JsonElement entity in conversationPrediction.GetProperty("entities").EnumerateArray())
                {
                    Console.WriteLine($"\t\tCategory: {entity.GetProperty("category").GetString()}");
                    Console.WriteLine($"\t\tText: {entity.GetProperty("text").GetString()}");
                    Console.WriteLine($"\t\tOffset: {entity.GetProperty("offset").GetInt32()}");
                    Console.WriteLine($"\t\tLength: {entity.GetProperty("length").GetInt32()}");
                    Console.WriteLine($"\t\tConfidence: {entity.GetProperty("confidenceScore").GetSingle()}");
                    Console.WriteLine();

                    if (entity.TryGetProperty("resolutions", out JsonElement resolutions))
                    {
                        foreach (JsonElement resolution in resolutions.EnumerateArray())
                        {
                            if (resolution.GetProperty("resolutionKind").GetString() == "DateTimeResolution")
                            {
                                Console.WriteLine($"\t\t\tDatetime Sub Kind: {resolution.GetProperty("dateTimeSubKind").GetString()}");
                                Console.WriteLine($"\t\t\tTimex: {resolution.GetProperty("timex").GetString()}");
                                Console.WriteLine($"\t\t\tValue: {resolution.GetProperty("value").GetString()}");
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }

            //Custom question answering response
            else if (targetIntentResult.GetProperty("targetProjectKind").GetString() == "QuestionAnswering")
            {
                JsonElement questionAnsweringResponse = targetIntentResult.GetProperty("result");


                Console.WriteLine("\tAnswers: \n");
                foreach (JsonElement answer in questionAnsweringResponse.GetProperty("answers").EnumerateArray())
                {
                    Console.WriteLine($"\t\t{answer.GetProperty("answer").GetString()}");
                    Console.WriteLine($"\t\tConfidence: {answer.GetProperty("confidenceScore")}");
                    Console.WriteLine($"\t\tSource: {answer.GetProperty("source")}");
                    Console.WriteLine();
                }

            }

            Console.ReadKey();
        }
    }
}
