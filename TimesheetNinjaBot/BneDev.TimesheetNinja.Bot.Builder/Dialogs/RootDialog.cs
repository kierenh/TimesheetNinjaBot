using BneDev.TimesheetNinja.Bot.Builder.Forms;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Runtime.Serialization;
using System.Web.Http;
using Autofac;
using System.Globalization;
using BneDev.TimesheetNinja.Bot.Builder.Model;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Bot.Builder.Properties;
using BneDev.TimesheetNinja.Common;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        [NonSerialized]
        private DialogServices dialogServices;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.dialogServices = Conversation.Container.Resolve<DialogServices>();
        }

        /// <summary>
        /// Whether LUIS is enabled for this dialog. 
        /// </summary>
        /// <remarks>
        /// A feature toggle to test mixing LUIS intents with plain old choices.
        /// When enabled everything is expected to come through as an intent. You can get to the user friendly menu via an intent, <see cref="LuisIntents.Menu"/>.        
        /// </remarks>
        public bool UseLuis { get; set; } = true;

        /// <summary>
        /// Whether the Bot is in demo mode (which, among other things simplifies authentication requirements).
        /// </summary>
        public bool IsDemoMode { get; set; }

        public RootDialog(DialogServices dialogServices)
            : base(dialogServices.LuisServices())
        {
            this.dialogServices = dialogServices;
        }

        public async override Task StartAsync(IDialogContext context)
        {
            if (UseLuis)
            {
                await base.StartAsync(context);
            }
            else
            {
                context.Wait(async (IDialogContext c, IAwaitable<object> result) =>
                {
                    await Greeting(c, true, true);
                });
            }
        }

        #region LUIS Intent Handlers - Timesheeting

        [LuisIntent(LuisIntents.AccessTokenSet)]
        public async Task SetAccessToken(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.SetSessionDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetAddTime)]
        public async Task AddTime(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.AddTimeDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetAddTimeStraightEights)]
        public async Task StraightEights(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.AddTimeDialog(result, AddTimeDialogMode.StraightEights);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetGetTime)]
        public async Task GetTime(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.GetTimeDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetGetDueDate)]
        public async Task CheckSubmissionStatus(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.CheckSubmissionStatusDialog();
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetGetProjectsUsedForTime)]
        public async Task GetProjects(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.GetProjectsDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.TimesheetAddProjectUsedForTime)]
        public async Task AddProject(IDialogContext context, LuisResult result)
        {
            // TODO:
            var dialog = new NotImplementedDialog();
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.ExpensesAddExpense)]
        public async Task AddExpense(IDialogContext context, LuisResult result)
        {
            var dialog = new NotImplementedDialog();
            // TODO:
            //var dialog = dialogServices.AddExpenseDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        [LuisIntent(LuisIntents.ExpensesGetExpenses)]
        public async Task GetExpenses(IDialogContext context, LuisResult result)
        {
            var dialog = dialogServices.GetExpensesDialog(result);
            context.Call(dialog, AfterDialog);
            await Task.CompletedTask;
        }

        #endregion

        #region  LUIS Intent Handlers - Help / Menu / Greet / None

        [LuisIntent("")]
        [LuisIntent("None")]
        [LuisIntent("Help")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            // Present the help for this bot-
            // It could also fallback-or breakout to QnA
            // http://www.garypretty.co.uk/2017/03/26/forwarding-activities-messages-to-other-dialogs-in-microsoft-bot-framework/
            // But QnA might be better suited to a menu option (i.e. breakout to QnA)

            Debug.WriteLine($"Detected intent: " + string.Join(", ", result.Intents.Select(i => i.Intent)));

            await context.PostAsync(Resources.Message_DidNotUnderstand);
            await Greeting(context, false, false);
        }

        [LuisIntent(LuisIntents.Greet)]
        public async Task Greet(IDialogContext context, LuisResult result)
        {
            await Greeting(context, true, false);
        }

        [LuisIntent(LuisIntents.MenuShow)]
        public async Task Menu(IDialogContext context, LuisResult result)
        {
            await Greeting(context, false, true);
        }

        [LuisIntent(LuisIntents.Undo)]
        public async Task Undo(IDialogContext context, LuisResult result)
        {
            // TODO: Look up Undo stack
            await context.PostAsync(Resources.Message_Undo);
        }

        #endregion

        private async Task Greeting(IDialogContext context, bool greet, bool showMenu)
        {
            var session = GetSession(context);
            bool isTokenActive = await IsAuthenticated(session);

            if (greet)
            {
                await context.PostAsync(Resources.Message_Greeting);
            }

            if (isTokenActive)
            {
                if (showMenu)
                {
                    PromptDialog.Choice(context, this.AfterTopMenuChoice, new[]
                    {
                        MenuIntents.CheckSubmissionStatus,
                        MenuIntents.GetProjects,
                        MenuIntents.GetTimesheet,
                        MenuIntents.LogTime,
                        MenuIntents.GetExpenses,
                        MenuIntents.AddExpense,
                        MenuIntents.Submit,
                        MenuIntents.Logout,
                        MenuIntents.ShowDisclaimer
                    }, Resources.Message_Menu_Authenticated, attempts: 2);
                }
                else
                {
                    await context.PostAsync(Resources.Message_Greeting_Authenticated);

                    context.Wait(MessageReceived);
                }
            }
            else
            {
                // Present a subset of the menu, guiding the user through setting an access token or logging in...
                PromptDialog.Choice(context, this.AfterTopMenuChoice, new[] { MenuIntents.SetAccessToken, MenuIntents.Login, MenuIntents.ShowDisclaimer }, Resources.Message_Greeting_NotAuthenticated, attempts: 3);
            }
        }

        protected virtual Session GetSession(IDialogContext context)
        {
            if (IsDemoMode)
            {
                var authService = this.dialogServices.TimesheetApis().AuthApi();
                var session = new Session()
                {
                    AuthToken = authService.AuthTokenFor(null).WaitAndUnwrapResult()
                };
                context.SetSession(session);
            }
            return context.GetSession();
        }

        public virtual async Task<bool> IsAuthenticated(Session session)
        {
            return IsDemoMode ? true : (session != null && await dialogServices.TimesheetApis().AuthApi().IsAccessTokenActive(session.AuthToken));
        }

        private async Task AfterTopMenuChoice(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = (await result).Trim();

                if (MenuIntents.SetAccessToken.IsMatch(selection))
                {
                    var setSessionDialog = dialogServices.SetSessionDialog();
                    context.Call(setSessionDialog, AfterDialog);
                }
                else if (MenuIntents.Login.IsMatch(selection))
                {
                    // TODO: Auth from a bot isn't a great experience, fundamentally, because there isn't a secure way to enter passwords
                    // The approach here is to pop open a browser to go through the auth flow to get an access token
                    // Store the access token in the bot state service
                    // Return a more simplstic code (i.e. 6-digit) to the user to paste in to the chat window
                    // which allows them to use that access token against MyTE APIs
                    context.Fail(new ApiException(Resources.Message_NotImplemented));
                }
                else if (MenuIntents.GetTimesheet.IsMatch(selection))
                {
                    context.Call(dialogServices.GetTimeDialog(), AfterDialog);
                }
                else if (MenuIntents.CheckSubmissionStatus.IsMatch(selection))
                {
                    context.Call(dialogServices.CheckSubmissionStatusDialog(), AfterDialog);
                }
                else if (MenuIntents.AddExpense.IsMatch(selection))
                {
                    context.Call(new NotImplementedDialog(), AfterDialog);
                    //TODO:
                    //context.Call(dialogServices.AddExpenseDialog(), AfterDialog);
                }
                else if (MenuIntents.GetProjects.IsMatch(selection))
                {
                    context.Call(dialogServices.GetProjectsDialog(), AfterDialog);
                }
                else if (MenuIntents.GetExpenses.IsMatch(selection))
                {
                    context.Call(dialogServices.GetExpensesDialog(), AfterDialog);
                }
                else if (MenuIntents.LogTime.IsMatch(selection))
                {
                    context.Call(dialogServices.AddTimeDialog(), AfterDialog);
                }
                else if (MenuIntents.Submit.IsMatch(selection))
                {
                    await context.PostAsync(Resources.Message_SubmitConfirmation);
                }
                else if (MenuIntents.Logout.IsMatch(selection))
                {
                    await context.PostAsync(Resources.Message_GoodBye);
                    context.SetSession(null);
                    context.Done((object)null);
                }
                else if (MenuIntents.ShowDisclaimer.IsMatch(selection))
                {
                    await context.PostAsync(Resources.Message_Disclaimer);
                }
            }
            catch (TooManyAttemptsException)
            {
                // Well, just go back to the start again anyway...
                await this.StartAsync(context);
            }
        }

        private async Task AfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception e)
            {
                await context.PostAsync(String.Format(CultureInfo.CurrentCulture, Resources.Message_DialogAborted, e.Message));
            }
            finally
            {
                await this.StartAsync(context);
            }
        }
    }
}
