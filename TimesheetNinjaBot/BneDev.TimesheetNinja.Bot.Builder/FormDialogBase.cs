using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Autofac;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    [Serializable]
    public abstract class FormDialogBase<T> : DialogBase<T>
        where T : class, new()
    {
        private T form = new T();
        protected T Form { get { return form; } set { form = value; } }

        public LuisResult LuisResult { get; private set; }

        public FormDialogBase(DialogServices dialogServices, LuisResult luisResult = null)
            : base(dialogServices)
        {
            this.LuisResult = luisResult;
        }

        protected async override Task MessageReceivedAsync(IDialogContext context)
        {
            await base.MessageReceivedAsync(context);

            var form = await BuildForm(context);
            context.Call(form, AfterForm);
        }

        protected virtual async Task<IFormDialog<T>> BuildForm(IDialogContext context)
        {
            var form = DialogExtensions.FromForm(() => DialogExtensions.CreateForm<T>()
                .AddRemainingFields()
                .Build(), form: this.Form, entities: LuisResult?.Entities);
            return await Task.FromResult(form);
        }

        /// <summary>
        /// Gets the resultant form if it was completed successfully (handling any errors that occur as well as the form being cancelled by the user).
        /// If the form is cancelled or errors then the context is updated appropriately to control the flow.
        /// </summary>
        /// <param name="context">The dialog context.</param>
        /// <param name="item">The form result.</param>
        /// <returns></returns>
        protected virtual async Task AfterForm(IDialogContext context, IAwaitable<T> item)
        {
            this.Form = await this.GetFormResult(context, item);

            if (this.Form != null)
            {
                try
                {
                    await ProcessForm(context);
                }
                catch (Exception e)
                {
                    await ProcessFormError(context, e);
                }
            }
        }

        /// <summary>
        /// Override this method to process the completed form. You are responsible for the chain/flow and ensuring <see cref="IDialogContext.Done"/> is called.
        /// Strictly speaking this should be limited to just processing the form result - if you want to call/chain other dialogs, then override <see cref="AfterForm(IDialogContext, IAwaitable{T})"/>.
        /// </summary>
        /// <param name="context">The dialog context.</param>
        /// <returns></returns>
        protected virtual async Task ProcessForm(IDialogContext context)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task ProcessFormError(IDialogContext context, Exception e)
        {
            if (await this.ProcessException(context, e))
            {
                System.Diagnostics.Debug.WriteLine("ProcessFormError - Clearing form state and restarting dialog...");
                // Clear dialog state and start again
                this.Form = new T();
                this.LuisResult = null;

                await StartAsync(context);
            }
        }
    }
}
