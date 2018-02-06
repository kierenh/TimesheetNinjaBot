using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BneDev.TimesheetNinja.Bot.Api;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinjaBot.Models
{
    public class ExpenseSheet : IExpenseSheet
    {
        public DateTime PeriodEnd { get; set; }

        public IEnumerable<ExpenseSheetLineItem> Expenses { get; set; }
        IEnumerable<IExpenseSheetLineItem> IExpenseSheet.Expenses { get => Expenses; set => this.Expenses = value.Cast<ExpenseSheetLineItem>(); }

        public (bool isPaid, int count, double total, double paid, double outstanding) IsPaid()
        {
            if (Expenses.Any())
            {
                var complete = this.Expenses.Where(x => String.Equals(x.ApprovalStatus, "paid", StringComparison.OrdinalIgnoreCase));

                var incomplete = this.Expenses.Except(complete);


                var isPaidModel = new
                {
                    isPaid = false,
                    total = this.Expenses.Sum(x => x.TotalAmount),
                    paid = complete.Sum(x => x.TotalAmount),
                    outstanding = incomplete.Sum(x => x.TotalAmount),
                };

                var count = Expenses.Count();
                return (isPaidModel.total == isPaidModel.paid, count, isPaidModel.total, isPaidModel.paid, isPaidModel.outstanding);
            }
            else
            {
                return (false, 0, 0, 0, 0);
            }
        }
    }
}