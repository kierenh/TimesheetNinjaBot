using Microsoft.Bot.Builder.Dialogs;
using BneDev.TimesheetNinja.Bot.Builder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    public static class DialogContextExtensions
    {
        public static Session GetSession(this IDialogContext context)
        {
            return context.UserData.TryGetValue(BotStateKeys.SessionId, out Session session) ? session : null;
        }

        public static void SetSession(this IDialogContext context, Session session)
        {
            if (session == null)
            {
                context.UserData.RemoveValue(BotStateKeys.SessionId);
            }
            else
            {
                context.UserData.SetValue(BotStateKeys.SessionId, session);
            }
        }
    }
}
