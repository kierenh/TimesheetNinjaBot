using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Common;
using System.Globalization;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    public class CheckSubmissionStatusDialog : DialogBase<object>
    {
        public CheckSubmissionStatusDialog(DialogServices dialogServices) : base(dialogServices)
        {
        }

        protected override async Task MessageReceivedAsync(IDialogContext context)
        {
            await base.MessageReceivedAsync(context);

            var session = context.GetSession();

            var timeService = this.DialogServices.TimesheetApis().TimeApi(session.AuthToken);
            var dateTimeService = this.DialogServices.CommonServices().DateTimeService();

            var periodEnd = dateTimeService.GetSubmissionPeriod(dateTimeService.Now());
            var timeSubmission = await timeService.IsSubmitted(periodEnd);

            var submissionStatus = timeSubmission.isSubmitted ? String.Format(CultureInfo.CurrentCulture, Resources.Message_MyTESubmittedConfirm, periodEnd) : String.Format(CultureInfo.CurrentCulture, Resources.Message_MyTESubmissionDeadline, periodEnd, timeSubmission.timesheet.Status);
            await context.PostAsync(submissionStatus);

            context.Done((object)null);
        }
    }
}
