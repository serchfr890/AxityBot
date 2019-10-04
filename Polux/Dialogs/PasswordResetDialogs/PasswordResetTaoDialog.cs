using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using CoreBot.Profiles;
using System.Threading;
using System.Globalization;

namespace CoreBot.Dialogs.PasswordResetDialogs
{
    public class PasswordResetTaoDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccesssor;
        private readonly ILogger<PasswordResetTaoDialog> _logger;
        public PasswordResetTaoDialog(UserState userState, ILogger<PasswordResetTaoDialog> logger) : base(nameof(PasswordResetTaoDialog))
        {
            _userProfileAccesssor = userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            var waterfallStepTao = new WaterfallStep[]
            {
                AskedEmpleoyeeNumberStepAsync,
                AskedAdmisionDateStepAsync,
                SendInformtionStepAsync,
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStepTao));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskedEmpleoyeeNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Perfecto!!!. Vamos a continuar con el cambio de contraseña de TAO, por favor comparte conmigo tu número de empledo.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskedAdmisionDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["EmployeeId"] = stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Por último, me puedes proporcionar tu fecha de admisión.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SendInformtionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["AdmisionDate"] = stepContext.Result;

            var userProfile = await _userProfileAccesssor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            try
            {
                userProfile.EmployeeId = Int32.Parse(stepContext.Values["EmployeeId"].ToString());
                userProfile.AdmisionDate = DateTime.Parse(stepContext.Values["AdmisionDate"].ToString(), new CultureInfo("es-MX")).ToString("dd-MM-yyyy");
            } catch (Exception e)
            {
                _logger.LogInformation($"Error en PasswordResetTaoDialog.cs {e.StackTrace}");
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
