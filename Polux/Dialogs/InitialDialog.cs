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

namespace CoreBot.Dialogs
{
    public class InitialDialog : ComponentDialog
    {
        private readonly BotAxityRecognizer _luisRecognizer;
        private readonly ILogger<InitialDialog> _logger;
        public InitialDialog(BotAxityRecognizer luisRecognizer, ILogger<InitialDialog> logger, PasswordResetSapDialog passwordResetSapDialog) : base (nameof(InitialDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;

            AddDialog(passwordResetSapDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                FinalStepAsync
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
                        //return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), cancellationToken);

                        string modulo;
                        try
                        {
                            modulo = luisResult.Entities.Modulo[0][0];
                        }
                        catch (Exception e)
                        {
                            modulo = "";
                        }

                        switch (modulo)
                        {
                            case "SAP":
                                return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), null, cancellationToken);
                                
                            case "AD":
                                break;
                            case "TAO":
                                break;
                            default:
                                await stepContext.Context.SendActivityAsync(MessageFactory.Text("OK, Por favor indicame el modulo"), cancellationToken);
                                break;
                        }
                        break;
                    case BotAxity.Intent.None:
                        break;
                }
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);



            /*else
            {

                 var luisResult = await _luisRecognizer.RecognizeAsync<BotAxity>(stepContext.Context, cancellationToken);
                 //return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), cancellationToken);

                 switch (luisResult.TopIntent().intent)
                 {
                     case BotAxity.Intent.PasswordReset:
                         return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), cancellationToken);

                    string modulo;
                    try
                    {
                        modulo = luisResult.Entities.Modulo[0][0];
                    } catch (Exception e)
                    {
                        modulo = "";
                    }

                    switch (modulo)
                    {
                        case "SAP":
                            return await stepContext.BeginDialogAsync(nameof(PasswordResetSapDialog), null, cancellationToken);
                            break;
                        case "AD":
                            break;
                        case "TAO":
                            break;
                        default:
                            await stepContext.Context.SendActivityAsync(MessageFactory.Text("OK, Por favor indicame el modulo"), cancellationToken);
                            break;
                    }
                    break;
                    case BotAxity.Intent.None:
                         break;
                 }*/


        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
