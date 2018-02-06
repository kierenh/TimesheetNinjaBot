using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    public static class DialogExtensions
    {
        public static async Task FormCanceled<TResult>(this IDialog<TResult> dialog, IDialogContext context, FormCanceledException e, string result = null)
        {
            string reply;

            if (e.InnerException == null)
            {
                reply = String.Format(CultureInfo.CurrentCulture, Resources.Message_FormCanceled);
            }
            else
            {
                reply = String.Format(CultureInfo.CurrentCulture, Resources.Message_DialogAborted, e.InnerException.Message);
            }

            await context.PostAsync(reply);
            context.Done(result);
        }

        /// <summary>
        /// Gets the form result.
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dialog"></param>
        /// <param name="context"></param>
        /// <param name="item"></param>
        /// <returns>The form result if successful; otherwise, <see cref="default(TForm)"/>.</returns>
        public static async Task<TForm> GetFormResult<TForm, TResult>(this IDialog<TResult> dialog, IDialogContext context, IAwaitable<TForm> item)
        {
            try
            {
                return await item;
            }
            catch (FormCanceledException<TForm> e)
            {
                // Note: It's possible to interogate the semi-processed form via
                // e.LastForm

                // User triggered cancel (i.e. type Quit)
                await dialog.FormCanceled(context, e);
            }
            catch (Exception e)
            {
                // Totally unrecoverable - bubble up to parent dialog to handle
                context.Fail(e);
            }
            return await Task.FromResult(default(TForm));
        }

        /// <summary>
        /// Processes the exception.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dialog"></param>
        /// <param name="context"></param>
        /// <param name="e"></param>
        /// <returns>returns <c>true</c> if the exception is handled; otherwise, <c>false</c>.</returns>
        public static async Task<bool> ProcessException<TResult>(this IDialog<TResult> dialog, IDialogContext context, Exception e)
        {
            if (e is ApiException)
            {
                // Api Exceptions are recoverable - i.e. invalid input, couldn't find order, etc.
                var errors = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(String.Empty, e.Message)
                };

                if (e is BusinessException)
                {
                    errors.AddRange((e as BusinessException).Errors);
                }

                var errorSummary = (from error in errors
                                    let field = error.Key
                                    let message = String.IsNullOrWhiteSpace(field) ? error.Value : $"{error.Key}: {error.Value}"
                                    select $"- {message}  ").ToArray(); // Note 2 spaces at end for markdown newline

                await context.PostAsync(String.Format(CultureInfo.CurrentCulture, Resources.Message_BusinessErrors, String.Join(Environment.NewLine, errorSummary)));

                return true;
            }
            else
            {
                // Totally unrecoverable - bubble up to parent dialog to handle
                context.Fail(e);

                return false;
            }
        }

        public static IFormDialog<T> FromForm<T>(BuildFormDelegate<T> buildForm, T form = null, FormOptions options = FormOptions.PromptInStart, IEnumerable<EntityRecommendation> entities = null)
            where T : class, new()
        {
            var dialog = new FormDialog<T>(form, buildForm, options, entities);
            return dialog;
        }

        public static DateTime? GetPeriodEnd<T>(this FormDialogBase<T> dialog, IDateTimeService dateTimeService, DateTime referenceDate)
            where T : class, new()
        {
            if (dialog.LuisResult == null || dialog.LuisResult.Entities.Count == 0)
            {
                // Prompt user for a period/date
                return null;
            }
            else
            {
                DateTime periodEnd;
                if (dialog.LuisResult.TryGetPeriod(dateTimeService, referenceDate, out periodEnd))
                {
                }
                else if (dialog.LuisResult.TryGetDate(out DateTime date))
                {
                    periodEnd = dateTimeService.GetSubmissionPeriod(date);
                }
                else
                {
                    periodEnd = dateTimeService.GetSubmissionPeriod(referenceDate);
                }
                return periodEnd;
            }
        }

        /// <summary>
        /// Factory method for creating forms consistently.
        /// </summary>
        /// <typeparam name="T">The form to create.</typeparam>
        /// <param name="ignoreAnnotations">True to ignore any attributes on the form class.</param>
        /// <returns></returns>
        public static IFormBuilder<T> CreateForm<T>(bool ignoreAnnotations = false)
            where T : class, new()
        {
            var form = new FormBuilder<T>(ignoreAnnotations);
            return form;
            // Leaving here as a reference point - good place to hook in if want to have the notion of global form customisation
            // such as adding additional terms to all forms to allow the user to get out or provide consistent help text
            var command = form.Configuration.Commands[FormCommand.Quit];
            var terms = command.Terms.ToList();
            terms.Add("get me outta here");
            command.Terms = terms.ToArray();
            var templateAttribute = form.Configuration.Template(TemplateUsage.NotUnderstood);
            var patterns = templateAttribute.Patterns;
            patterns[0] += "Type *cancel*, *back*, or *quit to quit or *help* if you want more information.";
            templateAttribute.Patterns = patterns;
            return form;
        }
    }
}
