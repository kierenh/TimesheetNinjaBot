using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;
using BneDev.TimesheetNinja.Common;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class ExpenseSheetLineItem : IExpenseSheetLineItem
    {
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ApprovalStatus { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }

        public string GetDateDescription()
        {
            return ToDate.HasValue ? $"{FromDate:dd/MM} - {ToDate.Value:dd/MM}" : $"{FromDate:dd/MM}";
        }
    }
}
