using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Bot.Api.Models
{
    public interface IExpenseSheet
    {
        IEnumerable<IExpenseSheetLineItem> Expenses { get; set; }

        (bool isPaid, int count, double total, double paid, double outstanding) IsPaid();
    }

    public interface IExpenseSheetLineItem
    {
        DateTime FromDate { get; set; }
        DateTime? ToDate { get; set; }
        string ApprovalStatus { get; set; }
        string Description { get; set; }
        double TotalAmount { get; set; }

        string GetDateDescription();
    }
}
