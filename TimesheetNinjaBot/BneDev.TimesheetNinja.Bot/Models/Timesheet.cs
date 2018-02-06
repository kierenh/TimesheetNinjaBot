using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class Timesheet : ITimesheet
    {
        public string EnterpriseId { get; set; }
        public string PeriodEnd { get; set; }
        public string Status { get; set; }
        public double ScheduledWorkHours { get; set; }

        public IEnumerable<TimesheetTask> Tasks { get; set; }

        IEnumerable<ITimesheetTask> ITimesheet.Tasks { get => Tasks; set => Tasks = value.Cast<TimesheetTask>(); }

        public IEnumerable<(string taskNumber, DateTime date, double hours)> GetDailyTimeEntries()
        {
            var dailyTimeEntries = from timesheetTask in Tasks
                                   from timeEntry in timesheetTask.TimeEntries
                                   let timEntryDate = TimesheetNinja.Common.DateTimeService.ParseIso8601Date(timeEntry.Date)
                                   select (timesheetTask.TaskNumber, timEntryDate, timeEntry.Hours);

            return dailyTimeEntries;
        }

        public double GetScheduledWorkHours()
        {
            return ScheduledWorkHours;
        }
    }
}
