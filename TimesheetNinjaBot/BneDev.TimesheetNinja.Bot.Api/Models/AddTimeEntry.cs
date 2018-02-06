using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api.Models
{
    public class AddTimeEntry
    {
        public DateTime PeriodEnd { get; set; }
        public DateTime ToDate { get; set; }

        public DateTime FromDate { get; set; }

        public string WbsCode { get; set; }

        public string WbsDescription { get; set; }

        public double Hours { get; set; }
    }
}
