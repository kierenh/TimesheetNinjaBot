using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api.Models
{
    public interface ITimeEntry
    {
        string Date { get; set; }
        double Hours { get; set; }
    }

    public interface ITimesheetTask
    {
        string TaskNumber { get; set; }
        string TaskDescription { get; set; }
        IEnumerable<ITimeEntry> TimeEntries { get; set; }
    }

    public interface ITimesheet
    {
        string EnterpriseId { get; set; }
        string PeriodEnd { get; set; }
        string Status { get; set; }
        IEnumerable<ITimesheetTask> Tasks { get; set; }

        IEnumerable<(string taskNumber, DateTime date, double hours)> GetDailyTimeEntries();

        double GetScheduledWorkHours();
    }
}
