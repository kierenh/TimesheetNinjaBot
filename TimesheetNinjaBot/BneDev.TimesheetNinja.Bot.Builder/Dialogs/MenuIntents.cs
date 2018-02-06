using BneDev.TimesheetNinja.Bot.Builder.Properties;
using System;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    public static class MenuIntents
    {
        public static readonly string SetAccessToken = Resources.Description_Menu_SetAccessToken;
        public static readonly string Login = Resources.Description_Menu_Login;
        public static readonly string ShowDisclaimer = Resources.Description_Menu_Disclaimer;
        public static readonly string GetTimesheet = Resources.Description_Menu_Timesheet;
        public static readonly string CheckSubmissionStatus = Resources.Description_CheckSubmissionStatus;
        public static readonly string Submit = Resources.Description_Menu_Submit;
        public static readonly string LogTime = Resources.Description_Menu_LogTime;
        public static readonly string AddExpense = Resources.Description_Menu_AddExpense;
        public static readonly string GetExpenses = Resources.Description_Menu_GetExpenses;

        public static readonly string Logout = Resources.Description_Menu_Logout;
        public static readonly string GetProjects = Resources.Description_Menu_GetProjects;
        public const string Nop = "No Op";
        // potentially: public const string QnA = "QnA";

        public static bool IsMatch(this string menuItemText, string input)
        {
            return String.Equals(input, menuItemText, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
