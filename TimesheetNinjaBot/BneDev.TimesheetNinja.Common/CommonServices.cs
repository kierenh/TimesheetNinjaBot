using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public class CommonServices
    {
        protected ILifetimeScope lifetimeScope;

        public CommonServices(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public ILogger Logger()
        {
            return lifetimeScope.Resolve<ILogger>();
        }

        public IDateTimeService DateTimeService()
        {
            return lifetimeScope.Resolve<IDateTimeService>();
        }

        public ConfigManager ConfigManager()
        {
            return lifetimeScope.Resolve<ConfigManager>();
        }
    }
}
