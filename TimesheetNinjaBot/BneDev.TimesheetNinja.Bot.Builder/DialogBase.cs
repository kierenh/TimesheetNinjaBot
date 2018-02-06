using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    [Serializable]
    public class DialogBase<T> : IDialog<T>
    {
        [NonSerialized]
        private DialogServices dialogServices;
        protected DialogServices DialogServices { get { return dialogServices; } }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.dialogServices = Conversation.Container.Resolve<DialogServices>();
        }

        public DialogBase(DialogServices dialogServices)
        {
            this.dialogServices = dialogServices;
        }

        public async virtual Task StartAsync(IDialogContext context)
        {
            await MessageReceivedAsync(context);
        }

        protected async virtual Task MessageReceivedAsync(IDialogContext context)
        {
            await Task.CompletedTask;
        }
    }
}
