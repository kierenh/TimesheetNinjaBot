using BneDev.TimesheetNinja.Bot.Builder.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using BneDev.TimesheetNinja.Bot.Builder;
using System.Globalization;
using BneDev.TimesheetNinja.Bot.Builder.Properties;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class GetExpensesDialog : FormDialogBase<GetExpenses>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetExpensesDialog"/> class.
        /// </summary>
        /// <param name="dialogServices">The dialog services.</param>
        /// <param name="luisResult">The LUIS result.</param>
        /// <param name="dialogMode">Whether the default period is the current period; otherwise, this is the previous period.</param>
        public GetExpensesDialog(DialogServices dialogServices, LuisResult luisResult = null) : base(dialogServices, luisResult)
        {
        }

        protected override async Task<IFormDialog<GetExpenses>> BuildForm(IDialogContext context)
        {
            var dateTimeService = DialogServices.CommonServices().DateTimeService();
            DateTime date;

            if (this.LuisResult != null)
            {
                // Infer period from tense
                if (this.LuisResult.TryGetExpensePeriod(dateTimeService, dateTimeService.Now(), out date))
                {
                    this.Form.PeriodEnd = date;
                }
                // Override with period inferred from date
                if (this.LuisResult.TryGetDate(out date))
                {
                    this.Form.PeriodEnd = dateTimeService.GetSubmissionPeriod(date);
                }
            }

            // Note: If PeriodEnd is null at this point, it will be prompted for via the form            

            return await base.BuildForm(context);
        }

        protected override async Task ProcessForm(IDialogContext context)
        {
            var session = context.GetSession();

            var expenseService = this.DialogServices.TimesheetApis().ExpenseApi(session.AuthToken);
            var dateTimeService = this.DialogServices.CommonServices().DateTimeService();

            // Use the entered period (if specified); otherwise, use the default which is either previous or current
            var periodEnd = dateTimeService.GetSubmissionPeriod(this.Form.PeriodEnd ?? dateTimeService.Now());
            var expenses = await expenseService.Get(periodEnd);
            var (isPaid, count, total, paid, outstanding) = expenses.IsPaid();

            string status;
            if (count == 0)
            {
                status = String.Format(CultureInfo.CurrentCulture, Resources.Message_ExpensesPaid_Zero, periodEnd);
            }
            else if (isPaid)
            {
                status = String.Format(CultureInfo.CurrentCulture, Resources.Message_ExpensesPaid, periodEnd, total);
            }
            else
            {
                var tableHeader = new[] { "| WBS| Amount| Date", "| ---| ---| ---" };
                var tableRows = expenses.Expenses.Select(x => "| " + String.Join("| ", x.Description, x.TotalAmount, x.GetDateDescription()));
                var table = tableHeader.Concat(tableRows);
                var tableSummary = String.Join($" |  {Environment.NewLine}", table);
                status = String.Format(CultureInfo.CurrentCulture, Resources.Message_ExpensesUnpaid, periodEnd, outstanding, total, tableSummary);
            }

            await context.PostAsync(status);
            context.Done(this.Form);

            await base.ProcessForm(context);
        }
    }
}
