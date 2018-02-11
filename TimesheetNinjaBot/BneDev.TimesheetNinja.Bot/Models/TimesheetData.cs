using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class TimesheetData
    {
        public IEnumerable<Timesheet> Timesheets { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public IEnumerable<ExpenseSheet> ExpenseSheets { get; set; }
    }
}
