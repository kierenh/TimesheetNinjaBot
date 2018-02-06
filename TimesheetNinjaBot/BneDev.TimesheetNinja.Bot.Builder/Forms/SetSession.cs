using Microsoft.Bot.Builder.FormFlow;
using BneDev.TimesheetNinja.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Bot.Builder.Forms
{
    [Serializable]

    public class SetSession
    {
        [Prompt("Paste in your {&}")]
        public String AccessToken { get; set; }
    }
}
