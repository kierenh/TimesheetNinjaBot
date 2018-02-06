using BneDev.TimesheetNinja.Bot.Builder.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using BneDev.TimesheetNinja.Bot.Builder;
using Microsoft.Bot.Builder.FormFlow;

namespace BneDev.TimesheetNinja.Bot.Builder.Dialogs
{
    [Serializable]
    public class AddExpenseDialog : FormDialogBase<AddExpense>
    {
        public AddExpenseDialog(DialogServices dialogServices, LuisResult luisResult = null) : base(dialogServices, luisResult)
        {
        }

        // TODO:
    }
}