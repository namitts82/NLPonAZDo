using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatWithAzDoBoard.Helper
{
    /// <summary>
    /// ADO Helper
    /// </summary>
    public class ADOHelper
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public ADOHelper(IConfiguration configuration, ILogger logger)
        {
            this.logger = logger;
            this.configuration = configuration;
        }
        /// <summary>
        /// Get work item summaries
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        async public Task<SearchResponse> GetWorkItemSummaries(string searchText)
        {
            try
            {
                var client = new HttpClient();
                var aDOAPIBaseURL = configuration["CollectionUri"];
                var aDOPAT = configuration["APIPat"];
                var top = int.Parse(configuration["TopResultsToConsider"]);
                var request = new HttpRequestMessage(HttpMethod.Post, aDOAPIBaseURL);
                request.Headers.Add("Authorization", string.Format("Basic {0}", aDOPAT));

                // Create a request object
                SerchRequest serchRequest = new SerchRequest(searchText);
                serchRequest.top = top;

                // Serialize the request object
                string requestString = JsonConvert.SerializeObject(serchRequest);
                var content = new StringContent(requestString, null, "application/json");
                request.Content = content;

                // Send the request
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Get the response
                var responseString = await response.Content.ReadAsStringAsync();
                SearchResponse searchResponse = JsonConvert.DeserializeObject<SearchResponse>(await response.Content.ReadAsStringAsync());

                // Get the engagement summary for each work item
                foreach (var wi in searchResponse.results)
                {
                    var id = wi.fields.systemid;
                    var engagementSummary = await GetEngagementSummaryFromADO(id);
                    wi.summary = engagementSummary;
                }
                return searchResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetWorkItemSummaries");
                throw;
            }   
        }
        /// <summary>
        /// Get engagement summary from ADO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<string> GetEngagementSummaryFromADO(string id)
        {
            try
            {
                var aDOBaseURL = configuration["BaseUri"];
                var aDOPAT = configuration["Pat"];
                var creds = new VssBasicCredential(string.Empty, aDOPAT);

                // Connect to Azure DevOps Services
                var connection = new VssConnection(new Uri(aDOBaseURL), creds);

                // Get a GitHttpClient to talk to the Git endpoints
                var adoClient = connection.GetClient<WorkItemTrackingHttpClient>();

                // Get the work item
                var workItem = await adoClient.GetWorkItemAsync(int.Parse(id), null, null, WorkItemExpand.Relations);

                // Set Title as a Heading
                string engagmentInfo = string.Format("\n# {0}\n", workItem.Fields["System.Title"].ToString());

                // Get the work item fields
                foreach (var field in workItem.Fields)
                {
                    if (field.Value != null)
                    {
                        // Get the field name (remove the System prefix)
                        var keySet = field.Key.Split(".");
                        var key = keySet[keySet.Length - 1];

                        string val;
                        if (field.Value is IdentityRef)
                        {
                            // Get the display name for People fields
                            IdentityRef resource = (IdentityRef)field.Value;
                            val = resource.DisplayName;
                        }
                        else
                        {
                            val = field.Value.ToString();
                        }
                        // Format the text in MD for better understanding by Open AI APIs
                        engagmentInfo += "* " + key + ":" + val + "\n";
                    }
                }
                return engagmentInfo;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEngagementSummaryFromADO");
                throw;
            }
        }
    }
}
