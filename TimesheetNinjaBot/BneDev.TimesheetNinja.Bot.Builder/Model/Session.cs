using BneDev.TimesheetNinja.Common;
using System;

namespace BneDev.TimesheetNinja.Bot.Builder.Model
{
    [Serializable]
    public class Session
    {
        public AuthToken AuthToken { get; set; }
    }
}