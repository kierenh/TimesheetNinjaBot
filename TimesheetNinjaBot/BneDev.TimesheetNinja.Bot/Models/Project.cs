using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class Project : IProject
    {
        public int SortOrder { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}