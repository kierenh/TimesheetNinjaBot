using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder.Forms
{
    [Serializable]
    public class AddExpense
    {
        public String WbsElement { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}