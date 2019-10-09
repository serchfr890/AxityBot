using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using CoreBot.CognitiveServices;
using Microsoft.Extensions.Logging;
using System.Threading;
using CoreBot.CognitiveModels;
using Microsoft.Bot.Schema;
using CoreBot.Dialogs.PasswordResetDialogs;
using CoreBot.Dialogs.FormDialog;
using Newtonsoft.Json;
using CoreBot.Models;
using System.IO;

namespace CoreBot.Dialogs
{
    public class InitialDialog : ComponentDialog
    {
        private readonly BotAxityRecognizer _luisRecognizer;
        private readonly ILogger<InitialDialog> _logger;
        public InitialDialog(BotAxityRecognizer luisRecognizer, ILogger<InitialDialog> logger, 
            PasswordResetSapDialog passwordResetSapDialog,
            PasswordResetTaoDialog passwordResetTaoDialog,
            PasswordResetAdDialog passwordResetAdDialog,
            FormDialogFromAdativeCard formDialogFromAdativeCard) : base (nameof(InitialDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(passwordResetSapDialog);
            AddDialog(passwordResetTaoDialog);
            AddDialog(passwordResetAdDialog);
            AddDialog(formDialogFromAdativeCard);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync
               // FinalStepAsync
            }));
            
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if ( !_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);
                
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }else
            {
                
                var luisResult = await _luisRecognizer.RecognizeAsync<BotAxity>(stepContext.Context, cancellationToken);
                switch (luisResult.TopIntent().intent)
                {
                    case BotAxity.Intent.PasswordReset:
                        string modulo;
                        try
                        { modulo = luisResult.Entities.Modulo[0][0]; }
                        catch (Exception e)
                        {
                            _logger.LogInformation($"Error en InitialDialog.cs {e.StackTrace}");
                            modulo = "";
                        }

                        switch (modulo)
                        {
                            case "SAP":
                                return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), null, cancellationToken);
                            case "AD":
                                return await stepContext.BeginDialogAsync(nameof(PasswordResetAdDialog), null, cancellationToken);
                            case "TAO":
                                return await stepContext.BeginDialogAsync(nameof(PasswordResetTaoDialog), null, cancellationToken);
                            default:
                                await SendSuggestedActionsModuleAsync(stepContext.Context, cancellationToken);
                                break;
                        }
                        break;
                    case BotAxity.Intent.ComprarProductos:
                        return await stepContext.BeginDialogAsync(nameof(FormDialogFromAdativeCard), null, cancellationToken);
                    case BotAxity.Intent.None:
                        break;
                }
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
        private static async Task SendSuggestedActionsModuleAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("OK, indícame el modulo por favor.");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "SAP", Type = ActionTypes.ImBack, Value = "SAP" },
                    new CardAction() { Title = "Active Directory", Type = ActionTypes.ImBack, Value = "AD" },
                    new CardAction() { Title = "Succces Factor", Type = ActionTypes.ImBack, Value = "TAO" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
