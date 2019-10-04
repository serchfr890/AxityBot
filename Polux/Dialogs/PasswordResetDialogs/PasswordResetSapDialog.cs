using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoreBot.Profiles;
using System.Threading;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace CoreBot.Dialogs.PasswordResetDialogs
{
    public class PasswordResetSapDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccesor;
        private readonly ILogger<PasswordResetSapDialog> _logger;

        public PasswordResetSapDialog(UserState userState, ILogger<PasswordResetSapDialog> logger) : base(nameof(PasswordResetSapDialog))
        {
            _userProfileAccesor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _logger = logger;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            var waterfallsStepsSap = new WaterfallStep[]
            {
                AskedUserStepAsync,
                AskedEmpleoyeeNumberStepAsync,
                AskedAdmisionDateStepAsync,
                AskedBirthDateStepAsync,
                SendInformtionStepAsync,
            };
            
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallsStepsSap));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskedUserStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Perfecto!!!. Vamos a continuar con el cambio de contraseña de SAP, por favor comparte conmigo tu usuario.")
            }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> AskedEmpleoyeeNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["UserId"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Ahora proporcióname tu número de empleado.")
            }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> AskedAdmisionDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["EmployeeId"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Proporcióname tu fecha de ingreso como muestra en el siguiente ejemplo: 01 de diciembre de 2012 o 01/12/2012 o 01-12-2012")
            }, cancellationToken);
            
        }

        private async Task<DialogTurnResult> AskedBirthDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["AdmisionDate"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Por último, comparteme tu fecha de nacimiento como se muestra en el siguiente ejemplo: 01 de diciembre de 2012 o 01/12/2012 o 01-12-2012")
            }, cancellationToken);
            
        }
        private async Task<DialogTurnResult> SendInformtionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["BirtDate"] = stepContext.Result;
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text(""), cancellationToken);
            var userProfile = await _userProfileAccesor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            try
            {
                userProfile.UserId = stepContext.Values["UserId"].ToString().ToLower();
                userProfile.EmployeeId = Int32.Parse(stepContext.Values["EmployeeId"].ToString());
                userProfile.AdmisionDate = DateTime.Parse(stepContext.Values["AdmisionDate"].ToString(), new CultureInfo("es-MX")).ToString("dd-MM-yyyy");
                userProfile.BirthDate = DateTime.Parse(stepContext.Values["BirtDate"].ToString(), new CultureInfo("es-MX")).ToString("dd-MM-yyyy");
            } catch (Exception e)
            {
                _logger.LogInformation($"Error SAP Dialog: {e.StackTrace}");
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Lo siento =( no puedo continuar con el proceso por qué " +
                    "algunos de tus datos son incorrectos"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Por favor vuelve a intentarlo o comunicate al 55555"), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Un momento por favor mientras hago el cambio de contraseña."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Te aviso cuando haya terminado el proceso"), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
    }
}
