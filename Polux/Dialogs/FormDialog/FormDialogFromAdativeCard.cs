using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreBot.Utils;

namespace CoreBot.Dialogs.FormDialog
{
    public class FormDialogFromAdativeCard : ComponentDialog
    {
        private readonly ILogger<FormDialogFromAdativeCard> _logger;
        private readonly BotState _userState;
        public FormDialogFromAdativeCard(ILogger<FormDialogFromAdativeCard> logger,
            UserState userState) : base(nameof(FormDialogFromAdativeCard))
        {
            _logger = logger;
            _userState = userState;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            var waterfallStepForm = new WaterfallStep[]
            {
                SendFormStepAsync,
                ReceiveFormStepAsync,
                SendInfoByEmailStepAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStepForm));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SendFormStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!string.Equals(stepContext.Context.Activity.Text.ToLower(), "no"))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Perfecto.!!! Por favor llena el siguiente formulario"), cancellationToken);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Attachments = new List<Attachment>() { CreateAdaptiveCardAttachment() },
                        Type = ActivityTypes.Message

                    }
                });
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("OK, He cancelado el formulario =( \n Espero que te comuniques pronto con nosotros"), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ReceiveFormStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Parsea de String a Json
            JObject json = JObject.Parse(stepContext.Result.ToString());
            var information = JsonConvert.DeserializeObject<Form>(JsonConvert.SerializeObject(json,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            if (!string.IsNullOrEmpty(information.Name) && !string.IsNullOrEmpty(information.Phone)
                && !string.IsNullOrEmpty(information.Email) && !string.IsNullOrEmpty(information.Comments))
            {
                stepContext.Values["Name"] = information.Name;
                stepContext.Values["Email"] = information.Email;
                stepContext.Values["Phone"] = information.Phone;
                stepContext.Values["Comments"] = information.Comments;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Muy bien, he recibido tus datos."), cancellationToken);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("¿Estas seguro de enviar este correo?") }, cancellationToken);
            } else
            {
                //Si uno de los campos está vacio, regresa al dialogo donde se manda el formulario
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Lo siento. Dejaste algunos campos vacios. ¿Quieres volver a llenar el formulario?") }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> SendInfoByEmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (string.Equals(stepContext.Context.Activity.Text.ToLower(), "yes"))
            {
                var name = stepContext.Values["Name"].ToString();
                var email = stepContext.Values["Email"].ToString();
                var phone = stepContext.Values["Phone"].ToString();
                var comments = stepContext.Values["Comments"].ToString();
                try
                {
                    SendMailToSigma.sendMail(name, email, phone, comments);
                }
                catch(ArgumentException e)
                {
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Hubo un error al enviar el correo. Por favor intentalo nuevamente: " + e.StackTrace), cancellationToken);
                    return await stepContext.EndDialogAsync();
                }
                
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("He enviado tu solicitud, tan pronto como sea posible, uno de nuestros asesores se comunicará contigo."), cancellationToken);
                return await stepContext.EndDialogAsync();
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("OK espero que te vuelvas a comunicar pronto."), cancellationToken);
                return await stepContext.EndDialogAsync();
            }
            
        }
        private Attachment CreateAdaptiveCardAttachment()
        {
            string[] paths = { ".", "Cards", "FormCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson)
            };
            return adaptiveCardAttachment;
        }
    }
}
