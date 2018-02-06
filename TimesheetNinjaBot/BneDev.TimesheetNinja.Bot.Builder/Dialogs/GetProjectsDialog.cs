using BneDev.TimesheetNinja.Bot.Builder.Properties;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    public class GetProjectsDialog : DialogBase<object>
    {
        public GetProjectsDialog(DialogServices dialogServices) : base(dialogServices)
        {
        }

        protected override async Task MessageReceivedAsync(IDialogContext context)
        {
            await base.MessageReceivedAsync(context);

            var session = context.GetSession();

            var projectService = this.DialogServices.TimesheetApis().ProjectApi(session.AuthToken);
            var projects = await projectService.GetAll();

            var projectsSummary = $"|Description |Code |  {Environment.NewLine}|--- |--- |{Environment.NewLine}  "
                + String.Join($" |  {Environment.NewLine}", projects.OrderBy(x => x.Description).Select((x, i) => $"|{x.Description} |{x.Code}"));

            await context.PostAsync(String.Format(CultureInfo.CurrentCulture, Resources.Message_GetProjectsSummary, projectsSummary));
            await context.PostAsync(String.Format(CultureInfo.CurrentCulture, Resources.Message_AddTimeHelp, "Wayne Enterprises", "A304515"));

            context.Done((object)null);
        }
    }
}
