using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder.Forms
{
    [Serializable]
    public class AddTime
    {
        [Template(TemplateUsage.Feedback, new[] { "OK GOT IT" })]
        [Template(TemplateUsage.NotUnderstood, new[] { "Sorry. I don't recognise that project.", "I need a valid project to assign your time to. If you didn't mean to assign time, type _back_ at anytime" })]
        public String Project { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public double? Hours { get; set; } // Default 8
    }
}
