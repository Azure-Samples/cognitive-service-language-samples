using System;
using System.Linq;
using Azure;
using Azure.AI.Language.Conversations;

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
            ConversationsProject orchestrationProject = new ConversationsProject("Orchestrator", "Testing");

            Console.WriteLine("Input a query to your orchestration project:");
            
            string query = Console.ReadLine();

            Response<AnalyzeConversationTaskResult> response = client.AnalyzeConversation(
                query,
                orchestrationProject);
            
            CustomConversationalTaskResult customConversationalTaskResult = response.Value as CustomConversationalTaskResult;
            var orchestratorPrediction = customConversationalTaskResult.Results.Prediction as OrchestratorPrediction;

            Console.WriteLine($"The top intent was {orchestratorPrediction.TopIntent}\n");
            Console.WriteLine($"The result from the connected project is as follows:\n");

            TargetIntentResult targetIntentResult = orchestratorPrediction.Intents[orchestratorPrediction.TopIntent];


            if(targetIntentResult.TargetKind == TargetKind.Conversation)
{
                ConversationTargetIntentResult cluTargetIntentResult = targetIntentResult as ConversationTargetIntentResult;

                ConversationResult conversationResult = cluTargetIntentResult.Result;
                ConversationPrediction conversationPrediction = conversationResult.Prediction;

                Console.WriteLine($"\tTop Intent: {conversationResult.Prediction.TopIntent}");
                Console.WriteLine($"\tIntents:");
                foreach (ConversationIntent intent in conversationPrediction.Intents)
                {
                    Console.WriteLine($"\t\tIntent Category: {intent.Category}");
                    Console.WriteLine($"\t\tConfidence: {intent.Confidence}");
                    Console.WriteLine();
                }

                Console.WriteLine($"\tEntities:");
                foreach (ConversationEntity entity in conversationPrediction.Entities)
                {
                    Console.WriteLine($"\t\tEntity Text: {entity.Text}");
                    Console.WriteLine($"\t\tEntity Category: {entity.Category}");
                    Console.WriteLine($"\t\tConfidence: {entity.Confidence}");
                    Console.WriteLine($"\t\tStarting Position: {entity.Offset}");
                    Console.WriteLine($"\t\tLength: {entity.Length}");
                    Console.WriteLine();

                    foreach (BaseResolution resolution in entity.Resolutions)
                    {
                        if (resolution is DateTimeResolution dateTimeResolution)
                        {
                            Console.WriteLine($"\t\t\tDatetime Sub Kind: {dateTimeResolution.DateTimeSubKind}");
                            Console.WriteLine($"\t\t\tTimex: {dateTimeResolution.Timex}");
                            Console.WriteLine($"\t\t\tValue: {dateTimeResolution.Value}");
                            Console.WriteLine();
                        }
                    }
                }
            }

            else if (targetIntentResult.TargetKind == TargetKind.QuestionAnswering)
            {
                QuestionAnsweringTargetIntentResult qnaTargetIntentResult = targetIntentResult as QuestionAnsweringTargetIntentResult;

                KnowledgeBaseAnswers qnaAnswers = qnaTargetIntentResult.Result;

                Console.WriteLine("\tAnswers: \n");
                foreach (KnowledgeBaseAnswer answer in qnaAnswers.Answers)
                {
                    Console.WriteLine($"\t\tAnswer: {answer.Answer}");
                    Console.WriteLine($"\t\tConfidence: {answer.Confidence}");
                    Console.WriteLine($"\t\tSource: {answer.Source}");
                    Console.WriteLine();
                }
            }


            Console.ReadKey();
        }
    }
}
