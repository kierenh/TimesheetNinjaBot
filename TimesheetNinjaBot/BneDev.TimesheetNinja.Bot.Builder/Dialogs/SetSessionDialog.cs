using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Bot.Builder.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using BneDev.TimesheetNinja.Bot.Builder.Model;
using System.Globalization;
using BneDev.TimesheetNinja.Common;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class SetSessionDialog : FormDialogBase<SetSession>
    {
        public SetSessionDialog(DialogServices dialogServices, LuisResult luisResult = null)
            : base(dialogServices, luisResult)
        {
        }

        protected override async Task ProcessForm(IDialogContext context)
        {
            var authToken = await this.DialogServices.TimesheetApis().AuthApi().AuthTokenFor(this.Form.AccessToken);

            var session = new Session()
            {
                AuthToken = authToken
            };
            var isTokenActive = await this.DialogServices.TimesheetApis().AuthApi().IsAccessTokenActive(authToken);

            if (isTokenActive)
            {
                context.SetSession(session);

                await context.PostAsync(String.Format(CultureInfo.CurrentCulture, Resources.Message_SetSessionSuccess, session.AuthToken.UserDisplayName));

                var dialog = DialogServices.CheckSubmissionStatusDialog();
                context.Call(dialog, async (IDialogContext c, IAwaitable<object> r) =>
                {
                    await c.PostAsync(Resources.Message_SetSessionSuccessHelpText);
                    c.Done(this.Form);
                });
            }
            else
            {
                throw new BusinessException("Access token is not valid. Copy-Paste error? Or has it expired? They normally expire after about 60 minutes of inactivity.");
            }

            await base.ProcessForm(context);
        }
    }
}
