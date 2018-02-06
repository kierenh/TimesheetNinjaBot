using Microsoft.Bot.Builder.FormFlow;
using BneDev.TimesheetNinja.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder.Forms
{
    [Serializable]
    public class GetTime
    {
        [Optional]
        [Template(TemplateUsage.NoPreference, new[] { "now", "current" })]
        public DateTime? PeriodEnd { get; set; }
    }
}
