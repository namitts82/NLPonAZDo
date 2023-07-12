using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;
using Microsoft.Bot.Schema;
using AdaptiveCards.Templating;

namespace ChatWithAzDoBoard.Helper
{
    public class AdaptiveCardsHelper
    {
        public Attachment CreateAdaptiveCardAttachment(string cardName, Object dataFeed)
        {
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(cardName));
            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    AdaptiveCardTemplate template = new AdaptiveCardTemplate(JsonConvert.DeserializeObject(adaptiveCard));
                    string cardJson = template.Expand(dataFeed);
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(cardJson),
                    };
                }
            }
        }
    }
}
