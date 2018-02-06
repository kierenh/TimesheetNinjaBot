using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class TimeEntry : ITimeEntry
    {
        public string Date { get; set; }
        public double Hours { get; set; }
    }
}
