using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Bot.Builder.Forms;
using BneDev.TimesheetNinja.Bot.Builder.Model;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Diagnostics;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Common;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class AddTimeDialog : FormDialogBase<AddTime>
    {
        public bool IsDemoMode { get; set; } = false;

        /// <summary>
        /// Strictly speaking this should be derived from the work schedule.
        /// </summary>
        public const int DailyHours = 8;

        IDictionary<string, IProject> projects;
        AddTimeDialogMode dialogMode;

        public AddTimeDialog(DialogServices dialogServices, LuisResult luisResult, AddTimeDialogMode dialogMode)
            : base(dialogServices, luisResult)
        {
            this.dialogMode = dialogMode;
        }

        protected override async Task<IFormDialog<AddTime>> BuildForm(IDialogContext context)
        {
            var session = context.GetSession();

            TrySetDateAndTime();

            await TrySetProjectOrProjects(session, context);

            var buildFormDelegate = new BuildFormDelegate<AddTime>(() =>
            {
                //var process = new OnCompletionAsyncDelegate<AddTime>(async (c, state) =>
                //{

                //});

                var formBuilder = DialogExtensions.CreateForm<AddTime>();
                //formBuilder.Configuration.DefaultPrompt.Feedback = FeedbackOptions.Default;

                return formBuilder.Field(new FieldReflector<AddTime>(nameof(AddTime.Project))
                    .SetType(null)
                    .SetActive(state => String.IsNullOrEmpty(state.Project))
                    .SetPrompt(new PromptAttribute("Please select a project: {||}")
                    {
                        ChoiceStyle = ChoiceStyleOptions.Buttons,
                        Feedback = FeedbackOptions.Auto
                    })
                    .SetDefine((state, field) =>
                    {
                        foreach (var item in this.projects.Values)
                        {
                            string[] terms;
                            if (String.IsNullOrWhiteSpace(item.Description))
                            {
                                terms = new[] { item.Code };
                            }
                            else
                            {
                                terms = new[] { item.Code, item.Description };
                            }
                            field
                                .AddDescription(item.Code, $"{item.Code} - {item.Description}")
                                .AddTerms(item.Code, terms);
                        }

                        return Task.FromResult(true);
                    }))
                .Field(nameof(AddTime.From))
                .Field(nameof(AddTime.To))
                .Field(nameof(AddTime.Hours))
                .AddRemainingFields()
                //.OnCompletion(process)
                .Build();
            });

            var form = DialogExtensions.FromForm(buildFormDelegate, form: this.Form);
            return form;
        }

        private void TrySetDateAndTime()
        {
            var dateTimeService = DialogServices.CommonServices().DateTimeService();

            if (dialogMode == AddTimeDialogMode.StraightEights)
            {
                // Infer all of these from the intent
                var now = dateTimeService.Now();
                var periodStart = dateTimeService.GetSubmissionPeriodStart(now);
                var periodEnd = dateTimeService.GetSubmissionPeriod(now);
                Form.From = periodStart;
                Form.To = periodEnd;
                Form.Hours = DailyHours;
            }
            else if (LuisResult != null)
            {
                // Assign from/to & hours - in order of precedence
                if (LuisResult.TryGetDateTimeRange(out (DateTime start, DateTime end, double? hours) dateTimeRange))
                {
                    // I worked on _x_ yesterday morning
                    Form.From = dateTimeRange.start;
                    Form.To = dateTimeRange.end;
                    Form.Hours = dateTimeRange.hours;
                }
                else
                {
                    // Try and set a timeframe
                    if (LuisResult.TryGetDateRange(out (DateTime start, DateTime end) dateRange))
                    {
                        // I worked on _x_ from 10/01 to 11/01
                        Form.From = dateRange.start;
                        Form.To = dateRange.end;
                    }
                    else if (LuisResult.TryGetDate(out DateTime date))
                    {
                        // I worked on _x_ yesterday
                        // I worked on _x_ on 10/01
                        Form.From = Form.To = date;
                        Form.Hours = DailyHours;
                    }
                    else
                    {
                        // No date present in utterance - assume today
                        var now = dateTimeService.Now();
                        Form.From = Form.To = now;
                    }

                    // Try and set a duration (interpretted as "each day") for each date in range (if set)
                    if (LuisResult.TryGetDuration(out TimeSpan duration))
                    {
                        // Log 8h on _x_
                        this.Form.Hours = duration.TotalHours;
                    }
                    else
                    {
                        // Hours not specified in utterance, default to today
                        this.Form.Hours = DailyHours;
                    }
                }
            }
        }

        private async Task TrySetProjectOrProjects(Session session, IDialogContext context)
        {
            var projectService = DialogServices.TimesheetApis().ProjectApi(session.AuthToken);
            var projects = await projectService.GetAll();

            // Try and set a WBS code / description
            if (LuisResult != null && LuisResult.TryGetWbs(out string wbs))
            {
                var projectSearchResults = ProjectSearch.FindAll(projects, wbs);
                if (projectSearchResults.Count() == 1)
                {
                    this.Form.Project = projectSearchResults.First().Code;
                }
                else
                {
                    projects = projectSearchResults;
                }
            }

            if (IsDemoMode && (String.IsNullOrEmpty(this.Form.Project) && projects.Count() > 5))
            {
                await context.PostAsync(Resources.Message_Demo_Projects);
                this.projects = projects.Take(5).ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                this.projects = projects.ToDictionary(x => x.Code, x => x, StringComparer.OrdinalIgnoreCase);
            }
        }

        protected override async Task ProcessForm(IDialogContext context)
        {
            var session = context.GetSession();
            var dateTimeService = DialogServices.CommonServices().DateTimeService();

            var model = new AddTimeEntry
            {
                FromDate = this.Form.From.Value.Date,
                ToDate = this.Form.To?.Date ?? this.Form.From.Value.Date,
                WbsCode = this.Form.Project,
                WbsDescription = this.projects[this.Form.Project].Description,
                Hours = this.Form.Hours.GetValueOrDefault(DailyHours)
            };
            model.PeriodEnd = dateTimeService.GetSubmissionPeriod(model.ToDate.Date);

            var timeService = DialogServices.TimesheetApis().TimeApi(session.AuthToken);

            if (dialogMode == AddTimeDialogMode.Default)
            {
                await timeService.Add(model);

                var dateMessage = (model.ToDate - model.FromDate).Days == 0 ? $"on {model.FromDate:dd/MM}" : $"from {model.FromDate:dd/MM} to {model.ToDate:dd/MM}";
                await context.PostAsync($"Assigning {model.Hours} hours to _{model.WbsDescription} ({model.WbsCode})_ {dateMessage} (period end: {model.PeriodEnd:ddd dd/MM}). Type _Undo_ if you'd like to revert this time entry.");
            }
            else
            {
                await timeService.StraightEights(model.PeriodEnd, model.WbsCode, model.WbsDescription, model.Hours);
                await context.PostAsync($"Assigning {model.Hours} hours to _{model.WbsDescription} ({model.WbsCode})_ for each day in your work schedule {model.PeriodEnd:ddd dd/MM}). Type _Undo_ if you'd like to revert this time entry.");
            }
            context.Done(this.Form);

            await base.ProcessForm(context);
        }
    }
}
