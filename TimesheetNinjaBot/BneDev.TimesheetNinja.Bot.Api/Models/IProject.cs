using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api.Models
{
    public interface IProject
    {
        int SortOrder { get; set; }

        string Code { get; set; }

        string Description { get; set; }
    }
}
