using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace CoreBot.Middleware
{
    public class Middleware1 : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("I run before every received activity"), cancellationToken);
            await next(cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text("I run after every received activity"), cancellationToken);
        }
    }
}
