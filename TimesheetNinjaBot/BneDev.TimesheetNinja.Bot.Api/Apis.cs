using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BneDev.TimesheetNinja.Common;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public class Apis : IApis
    {
        private ILifetimeScope lifetimeScope;

        public Apis(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public IExpenseApi ExpenseApi(AuthToken authToken)
        {
            var parameters = new[]
            {
                new NamedParameter(nameof(authToken), authToken)
            };
            return this.lifetimeScope.Resolve<IExpenseApi>(parameters);
        }

        public IProjectApi ProjectApi(AuthToken authToken)
        {
            var parameters = new[]
            {
                new NamedParameter(nameof(authToken), authToken)
            };
            return this.lifetimeScope.Resolve<IProjectApi>(parameters);
        }

        public ITimeApi TimeApi(AuthToken authToken)
        {
            var parameters = new[]
            {
                new NamedParameter(nameof(authToken), authToken)
            };
            return this.lifetimeScope.Resolve<ITimeApi>(parameters);
        }

        public IAuthApi AuthApi()
        {
            return this.lifetimeScope.Resolve<IAuthApi>();
        }
    }
}
