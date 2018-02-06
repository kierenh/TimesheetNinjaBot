using BneDev.TimesheetNinja.Bot.Builder.Properties;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    public class NotImplementedDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Resources.Message_NotImplemented);
            context.Done((object)null);
        }
    }
}