using System;
using System.Threading.Tasks;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public interface IExpenseApi
    {
        Task<IExpenseSheet> Get(DateTime periodEnd);
    }
}
