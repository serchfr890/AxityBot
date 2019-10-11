using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CoreBot.Dialogs.TicketReview
{
    public class TicketReviewDialog : ComponentDialog 
    {
        public TicketReviewDialog() : base(nameof(TicketReviewDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            var waterfallTicketReviewStep = new WaterfallStep[]
            {
                 AskedIdTicketStepAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallTicketReviewStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskedIdTicketStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //TODO Implementar la lógica del dialogo
            var Id_Ticket = stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entro al dialogo de Ticket review"), cancellationToken);
            return await stepContext.EndDialogAsync();
        }
    }
}
