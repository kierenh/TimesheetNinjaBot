using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    public static class LuisIntents
    {
        public const string AccessTokenSet = "AccessToken.Set";
        public const string Greet = "Greet";
        public const string MenuShow = "Menu.Show";
        public const string TimesheetAddTime = "Timesheet.AddTime";
        public const string TimesheetAddTimeStraightEights = "Timesheet.AddTimeStraightEights";
        public const string TimesheetGetDueDate = "Timesheet.GetDueDate";
        public const string TimesheetGetTime = "Timesheet.GetTime";
        public const string TimesheetGetProjectsUsedForTime = "Timesheet.GetProjectsUsedForTime";
        public const string TimesheetAddProjectUsedForTime = "Timesheet.AddProjectsUsedForTime";
        public const string TimesheetSubmit = "Timesheet.Submit";
        public const string ExpensesGetExpenses = "Expenses.GetExpenses";
        public const string ExpensesAddExpense = "Expenses.AddExpense";
        public const string Undo = "Undo";
    }
}
