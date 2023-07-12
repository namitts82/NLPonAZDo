using AI.Dev.OpenAI.GPT;
using Azure;
using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatWithAzDoBoard.Helper
{
    public class OpenAIHelper
    {
        private OpenAIClient client;
        private int maxTokenCount = 4000 - responseMaxTokens;
        private const int responseMaxTokens = 256;
        private readonly string completionModelName;
        private readonly string searchKeywordModelName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionKey"></param>
        /// <param name="baseUri"></param>
        /// <param name="completionModelName"></param>
        /// <param name="searchKeywordModelName"></param>
        public OpenAIHelper(string subscriptionKey, string baseUri, string completionModelName, string searchKeywordModelName)
        {
            this.completionModelName = completionModelName;
            this.searchKeywordModelName = searchKeywordModelName;
            client = new OpenAIClient(
                new Uri(baseUri),
                new AzureKeyCredential(subscriptionKey));
        }

        /// <summary>
        /// Strips HTML tags
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string StripHtml(string html)
        {
            // Replace all HTML tags with a newline.
            return new Regex(@"</?.+?>").Replace(html, "\n");
        }
        /// <summary>
        /// Get response from LLM model based on the feed in string and question
        /// </summary>
        /// <param name="feedInString"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(string feedInString, bool includeTotal = false)
        {
            string contentString = string.Empty;
            string returnString = string.Empty;
            int maxTokenCount = 4000;

            string prompt;
            if(includeTotal)
                prompt = "{0}\n Answer the following question from the text above. \nQ: What was the total number of results found and Create a presentable summary of the text above\nA: ";
            else 
                prompt = "{0}\n Answer the following question from the text above. \nQ: Create a short presentable summary of the text above\nA: ";

            var promptString = string.Format(prompt, feedInString);

            // Check for total token count
            List<int> tokens = GPT3Tokenizer.Encode(promptString);
            if (tokens.Count >= maxTokenCount)
            {
                // Truncate the string to 60% of the max token count
                promptString = promptString.Substring(0,(int)(0.6 * maxTokenCount));
            }

            try
            {
                Response<Completions> completionsResponse = await client.GetCompletionsAsync(
                                                    deploymentOrModelName: completionModelName,
                                                    new CompletionsOptions()
                                                    {
                                                        Prompts = { promptString },
                                                        Temperature = (float)0.7,
                                                        MaxTokens = 256,
                                                        StopSequences = { "\n" },
                                                        NucleusSamplingFactor = (float)1,
                                                        FrequencyPenalty = (float)0,
                                                        PresencePenalty = (float)0,
                                                        GenerationSampleCount = 1,
                                                    });
                Completions completions = completionsResponse.Value;
                return completions.Choices[0].Text;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get the key entities from the user search string
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<string> GetKeyEntities(string promptTemplate, string question)
        {
            // Add question to the content string
            var prompt = string.Format(promptTemplate, question);
            // Call API
            Response<Completions> completionsResponse = await client.GetCompletionsAsync(
                                                    deploymentOrModelName: searchKeywordModelName,
                                                    new CompletionsOptions()
                                                    {
                                                        Prompts = { prompt },
                                                        Temperature = (float)0.7,
                                                        MaxTokens = 256,
                                                        StopSequences = { "\n" },
                                                        NucleusSamplingFactor = (float)1,
                                                        FrequencyPenalty = (float)0,
                                                        PresencePenalty = (float)0,
            });
            Completions completions = completionsResponse.Value;
            // Add the first choice to the return string
            return completions.Choices[0].Text;
        }

    }

}
