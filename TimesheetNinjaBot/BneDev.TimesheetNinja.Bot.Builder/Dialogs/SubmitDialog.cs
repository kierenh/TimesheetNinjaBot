using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;

namespace MyTEApi.Bot.Dialogs
{
    public class SubmitDialog : DialogBase<object>
    {
        public SubmitDialog(DialogServices dialogServices) : base(dialogServices)
        {
        }

        protected override Task MessageReceivedAsync(IDialogContext context)
        {
            return base.MessageReceivedAsync(context);
        }
    }
}
