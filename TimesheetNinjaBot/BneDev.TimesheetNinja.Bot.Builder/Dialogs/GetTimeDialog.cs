using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Bot.Builder.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Globalization;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class GetTimeDialog : FormDialogBase<GetTime>
    {
        public GetTimeDialog(DialogServices dialogServices, LuisResult luisResult)
            : base(dialogServices, luisResult)
        {
        }

        protected override async Task<IFormDialog<GetTime>> BuildForm(IDialogContext context)
        {
            var dateTimeService = DialogServices.CommonServices().DateTimeService();
            this.Form.PeriodEnd = this.GetPeriodEnd(dateTimeService, dateTimeService.Now());
            return await base.BuildForm(context);
        }

        protected override async Task ProcessForm(IDialogContext context)
        {
            var session = context.GetSession();

            var timeService = this.DialogServices.TimesheetApis().TimeApi(session.AuthToken);
            var dateTimeService = this.DialogServices.CommonServices().DateTimeService();

            var periodEnd = dateTimeService.GetSubmissionPeriod(this.Form.PeriodEnd ?? dateTimeService.Now());
            var time = await timeService.Get(periodEnd);

            var dailyTimeEntries = time.GetDailyTimeEntries();

            var hoursWorked = dailyTimeEntries.Sum(x => x.hours);
            var workScheduleHours = time.GetScheduledWorkHours();

            var summaryText = String.Format(CultureInfo.CurrentCulture, Resources.Message_PeriodSummaryText, periodEnd, time.Status, hoursWorked, workScheduleHours);
            if (dailyTimeEntries.Any())
            {
                var tableHeader = new[] { " WBS| Date| Hours", "---| ---| ---" };
                var tableRows = dailyTimeEntries.Any() ? dailyTimeEntries.Select(y => String.Join("| ", y.taskNumber, y.date.ToString("dd/MM", CultureInfo.CurrentCulture), y.hours)) : Enumerable.Empty<string>();
                var table = tableHeader.Concat(tableRows);
                await context.PostAsync(summaryText + $"{Environment.NewLine}  | " + String.Join($" |  {Environment.NewLine}", table));
            }
            else
            {
                await context.PostAsync(summaryText);
            }

            context.Done(this.Form);

            await base.ProcessForm(context);
        }
    }
}
