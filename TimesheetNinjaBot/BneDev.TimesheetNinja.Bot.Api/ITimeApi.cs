using BneDev.TimesheetNinja.Bot.Api.Models;
using System;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public interface ITimeApi
    {
        Task Add(AddTimeEntry addTimeEntry);

        Task<ITimesheet> Get(DateTime periodEnd);

        Task<(ITimesheet timesheet, bool isSubmitted)> IsSubmitted(DateTime referenceDate);

        Task<ITimesheet> StraightEights(DateTime periodEnd, string taskNumber, string taskDescription, double hours = 8, bool submit = false);

        Task Submit();
    }
}
