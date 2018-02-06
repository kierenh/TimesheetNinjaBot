using BneDev.TimesheetNinja.Bot.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public interface IProjectApi
    {
        Task Add(string projectCode);

        Task<IEnumerable<IProject>> GetAll();
    }
}