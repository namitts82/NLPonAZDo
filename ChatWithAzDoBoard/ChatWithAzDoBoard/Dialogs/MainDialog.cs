using ChatWithAzDoBoard.Helper;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.Work.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatWithAzDoBoard.Dialogs
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class MainDialog : ComponentDialog
    {
        // Dependency injection uses this constructor to instantiate MainDialog
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        public ILogger Logger => logger;
        public IConfiguration Configuration => configuration;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(IConfiguration configuration,
             UserState userState,ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            this.logger = logger;
            this.configuration = configuration;

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            // Define the main dialog and its related components.
            var waterfallSteps = new WaterfallStep[]
            {
                    IntroStepAsync,
                    FindEntitiesStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// Intro step for the dialog
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = "Type you rquery in natural language";
            // Create the PromptOptions which contain the prompt and re-prompt messages.
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        /// <summary>
        /// Final step in the dialog. This will run after the user has responded to the prompt.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> FindEntitiesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resSummary = string.Empty;

            // Get Open AI service credentials
            OpenAIHelper oAIhelper = new OpenAIHelper(Configuration["OpenAISubscriptionKey"], Configuration["OpenAIEndPoint"], Configuration["TextCompletionModelName"], Configuration["SearchKeywordModelName"]);
            
            // Load the prompt template
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith("adosyntax.md"));
            var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath);
            var reader = new StreamReader(stream);            
            var promptTemplate = reader.ReadToEnd();

            // Get the keywords for the search
            resSummary = "User Query: " + stepContext.Context.Activity.Text.Trim();
            var searchTerms = await oAIhelper.GetKeyEntities(promptTemplate, stepContext.Context.Activity.Text.Trim());
            await stepContext.Context.SendActivityAsync(string.Format("Found the following keywords of interest to search for: {0}", searchTerms));

            resSummary += string.Format("\nFound the following keywords of interest to search for: {0}. Searching ADO ⏳", searchTerms) ;

            // Search ADO for the keywords
            ADOHelper aDOhelper = new ADOHelper(Configuration, Logger);
            var response = await aDOhelper.GetWorkItemSummaries(searchTerms);

            resSummary += string.Format("\nTotal nubmer of results found was {0} for the search query from Azure Dev Ops", response.count);

            // Get the top responses from Open AI
            if (response.count > 0)
            {
                int maxResults = 3;

                resSummary += "\nHere are the top results found for your query: \n";
                ResponseCard responseCard = new ResponseCard();
                responseCard.total = response.results.Count.ToString();
                responseCard.workitem = new List<Workitem>();

                var maxTasksToShow = response.results.Count>10?10:response.results.Count;
                for(int i=0; i< maxTasksToShow;i++)
                {
                    responseCard.workitem.Add(new Workitem() { name = response.results[i].fields.systemtitle, id = response.results[i].fields.systemid, link = response.results[i].url });
                }

                var cardsHelper = new AdaptiveCardsHelper();
                var adoCard = cardsHelper.CreateAdaptiveCardAttachment("wilistCard.json", responseCard);
                var card = MessageFactory.Attachment(adoCard, ssml: "Found Task!");
                await stepContext.Context.SendActivityAsync(card, cancellationToken);

                responseCard.workitem = new List<Workitem>();

                foreach (var wi in response.results)
                {
                    if (maxResults > 0)
                    {
                        var curRes = await oAIhelper.GetResponse(wi.summary);
                        if (!string.IsNullOrEmpty(curRes))
                        {
                            resSummary += "\n# " + wi.fields.systemtitle;
                            resSummary += "\n";
                            resSummary += curRes;
                            resSummary += "\n";                           
                            maxResults--;
                            responseCard.workitem.Add(new Workitem() { name = wi.fields.systemtitle, itemsummary = curRes, link = wi.url });
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                var finalRes = await oAIhelper.GetResponse(resSummary,true);
                responseCard.summary = finalRes;

                adoCard = cardsHelper.CreateAdaptiveCardAttachment("resultCard.json", responseCard);
                card = MessageFactory.Attachment(adoCard, ssml: "Found Task!");
                await stepContext.Context.SendActivityAsync(card, cancellationToken);
            }
            // Restart the dialog
            stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}
