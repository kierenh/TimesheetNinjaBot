using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class TimesheetTask : ITimesheetTask
    {
        public string TaskNumber { get; set; }
        public string TaskDescription { get; set; }
        public IEnumerable<TimeEntry> TimeEntries { get; set; }
        IEnumerable<ITimeEntry> ITimesheetTask.TimeEntries { get => TimeEntries; set => this.TimeEntries = value.Cast<TimeEntry>(); }
    }
}
