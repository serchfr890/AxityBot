using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace CoreBot.CognitiveServices
{
    public class BotAxityRecognizer : IRecognizer
    {
        private readonly LuisRecognizer _luisRecognizer;
        private readonly ILogger<BotAxityRecognizer> _logger;
        public BotAxityRecognizer(IConfiguration configuration, ILogger<BotAxityRecognizer> logger)
        {
            _logger = logger;
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) && !string.IsNullOrEmpty(configuration["LuisAPIKey"]) && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);

            if(luisIsConfigured)
            {
                var luisApplication = new LuisApplication(configuration["LuisAppId"], configuration["LuisAPIKey"], "https://" + configuration["LuisAPIHostName"]);
                _luisRecognizer = new LuisRecognizer(luisApplication);
            }
        }

        public virtual bool IsConfigured => _luisRecognizer != null;


        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
            => await _luisRecognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}
