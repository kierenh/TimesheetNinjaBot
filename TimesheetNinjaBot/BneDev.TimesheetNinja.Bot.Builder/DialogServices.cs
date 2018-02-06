using Autofac;
using Autofac.Core;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using BneDev.TimesheetNinja.Bot.Builder.Dialogs;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using BneDev.TimesheetNinja.Bot.Api;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    /// <summary>
    /// A segregated service locator for resolving "dialog" components.
    /// </summary>
    public class DialogServices
    {
        private ILifetimeScope lifetimeScope;

        public DialogServices(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public ILuisService[] LuisServices()
        {
            return lifetimeScope.Resolve<IEnumerable<ILuisService>>().ToArray();
        }

        public IApis TimesheetApis()
        {
            return lifetimeScope.Resolve<IApis>();
        }

        public CommonServices CommonServices()
        {
            return lifetimeScope.Resolve<CommonServices>();
        }

        public IDialog<object> AddTimeDialog(LuisResult luisResult = null, AddTimeDialogMode dialogMode = AddTimeDialogMode.Default)
        {
            var parameters = new Parameter[]
            {
                new NamedParameter(nameof(luisResult), luisResult),
                new NamedParameter(nameof(dialogMode), dialogMode)
            };
            return lifetimeScope.Resolve<AddTimeDialog>(parameters);
        }

        public IDialog<object> RootDialog()
        {
            return lifetimeScope.ResolveNamed<IDialog<object>>("RootDialog");
        }

        public IDialog<object> SetSessionDialog(LuisResult luisResult = null)
        {
            var parameters = new Parameter[]
            {
                new NamedParameter(nameof(luisResult), luisResult)
            };
            return lifetimeScope.Resolve<SetSessionDialog>(parameters);
        }

        public IDialog<object> GetTimeDialog(LuisResult luisResult = null)
        {
            var parameters = new Parameter[]
            {
                new NamedParameter(nameof(luisResult), luisResult)
            };
            return lifetimeScope.Resolve<GetTimeDialog>(parameters);
        }

        public IDialog<object> CheckSubmissionStatusDialog()
        {
            return lifetimeScope.Resolve<CheckSubmissionStatusDialog>();
        }

        public IDialog<object> GetExpensesDialog(LuisResult luisResult = null)
        {
            var parameters = new Parameter[]
            {
                new NamedParameter(nameof(luisResult), luisResult)
            };
            return lifetimeScope.Resolve<GetExpensesDialog>(parameters);
        }

        public IDialog<object> AddExpenseDialog(LuisResult luisResult = null)
        {
            var parameters = new Parameter[]
            {
                new NamedParameter(nameof(luisResult), luisResult)
            };
            return lifetimeScope.Resolve<AddExpenseDialog>(parameters);
        }

        internal IDialog<object> GetProjectsDialog(LuisResult luisResult = null)
        {
            var parameters = new Parameter[]
           {
                new NamedParameter(nameof(luisResult), luisResult)
           };
            return lifetimeScope.Resolve<GetProjectsDialog>(parameters);
        }
    }
}
