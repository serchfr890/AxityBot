using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoreBot.CognitiveServices;
using Microsoft.Extensions.Logging;
using System.Threading;
using CoreBot.CognitiveModels;
using Microsoft.Bot.Schema;
using CoreBot.Dialogs.PasswordResetDialogs;
using CoreBot.Dialogs.FormDialog;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.AI.QnA;
using Newtonsoft.Json.Linq;
using CoreBot.Dialogs.TicketReview;

namespace CoreBot.Dialogs
{
    public class InitialDialog : ComponentDialog
    {
        private readonly BotAxityRecognizer _luisRecognizer;
        private readonly ILogger<InitialDialog> _logger;
        private readonly IConfiguration _configuration;

        public InitialDialog(BotAxityRecognizer luisRecognizer,
            ILogger<InitialDialog> logger,
            IConfiguration configuration,
            PasswordResetSapDialog passwordResetSapDialog,
            PasswordResetTaoDialog passwordResetTaoDialog,
            PasswordResetAdDialog passwordResetAdDialog,
            FormDialogFromAdativeCard formDialogFromAdativeCard,
            TicketReviewDialog ticketReviewDialog) : base (nameof(InitialDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;
            _configuration = configuration;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(passwordResetSapDialog);
            AddDialog(passwordResetTaoDialog);
            AddDialog(passwordResetAdDialog);
            AddDialog(formDialogFromAdativeCard);
            AddDialog(ticketReviewDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync
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
                var answerFromService = " ";
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
                    case BotAxity.Intent.QnAMakerSigma:
                        //No colocar el IhttpFactoryclient

                        var qnaMaker = new QnAMaker(new QnAMakerEndpoint
                        {
                            KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                            EndpointKey = _configuration["QnAAuthKey"],
                            Host = _configuration["QnAEndpointHostName"]
                        });

                        var results = await qnaMaker.GetAnswersAsync(stepContext.Context);
                        if (results != null && results.Length > 0)
                        {
                            try
                            {
                                JObject resultToJson = JObject.Parse(results[0].Answer);
                                var answers = resultToJson.SelectToken("Message").ToList();
                                foreach (var answer in answers)
                                {
                                    await stepContext.Context.SendActivityAsync(MessageFactory.Text(answer.ToString()), cancellationToken);
                                }
                            } catch
                            {
                                await stepContext.Context.SendActivityAsync(MessageFactory.Text(results[0].Answer), cancellationToken);
                            }
                            
                        }
                        break;
                    case BotAxity.Intent.TicketReview:
                        try
                        {
                            var Id_ticket = luisResult.Entities.Id_Ticket[0];
                            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Perfecto, voy a checarlo con el equipo, en breve te tendré una respuesta"), cancellationToken);
                            await stepContext.BeginDialogAsync(nameof(TicketReviewDialog), Id_ticket, cancellationToken);
                        } catch
                        {
                            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ok, pero olvidaste ingresar el Id_Ticket"), cancellationToken);
                            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Vuelve a intentarlo como el siguiente ejemplo \"Dime el estado de mi ticket 234NHJ2r\""), cancellationToken);
                        }
                        break;
                    case BotAxity.Intent.Pregunta_1:
                        var DESC_ENTITY = luisResult.Entities.DESC_CUENTA[0];
                        var msg = MessageFactory.Text("Pregunta 1");
                        msg.Speak = "< speak version = \"1.0\" xmlns = \"https://www.w3.org/2001/10/synthesis\" xml: lang = \"es-MX\" >HOla perro</ speak >";
                              await stepContext.Context.SendActivityAsync(msg, cancellationToken);
                        




                        answerFromService = getAnswerFromQuestion1();
                        break;
                    case BotAxity.Intent.Pregunta_2:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 2"), cancellationToken);
                        answerFromService = getAnswerFromQuestion2();
                        break;
                    case BotAxity.Intent.Pregunta_3:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 3"), cancellationToken);
                        answerFromService = getAnswerFromQuestion3();
                        break;
                    case BotAxity.Intent.Pregunta_4:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 4"), cancellationToken);
                        answerFromService = getAnswerFromQuestion4();
                        break;
                    case BotAxity.Intent.Pregunta_5:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 5"), cancellationToken);
                        answerFromService = getAnswerFromQuestion5();
                        break;
                    case BotAxity.Intent.Pregunta_6:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 6"), cancellationToken);
                        answerFromService = getAnswerFromQuestion6();
                        break;
                    case BotAxity.Intent.Pregunta_7:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Entró a Pregunta 7"), cancellationToken);
                        answerFromService = getAnswerFromQuestion7();
                        break;
                    default:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Lo siento no entendí, vuelve a intentarlo."), cancellationToken);
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

        private static string getAnswerFromQuestion1()
        {
            return "pregunta1";
        }

        private static string getAnswerFromQuestion2()
        {
            return "pregunta2";
        }

        private static string getAnswerFromQuestion3()
        {
            return "pregunta3";
        }

        private static string getAnswerFromQuestion4()
        {
            return "pregunta4";
        }

        private static string getAnswerFromQuestion5()
        {
            return "pregunta5";
        }

        private static string getAnswerFromQuestion6()
        {
            return "pregunta6";
        }

        private static string getAnswerFromQuestion7()
        {
            return "pregunta7";
        }

        public IActivity Speak(string message)
        {
            var activity = MessageFactory.Text(message);
            string body = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='es-MX'> 

        <voice name='Microsoft Server Speech Text to Speech Voice (es-MX, JessaRUS)'>" +
                $"{message}" + "</voice></speak>";

            activity.Speak = body;
            return activity;
        }
    }
}
