using Autofac;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public class TimesheetNinjaBotApiModule : ModuleBase
    {
        public TimesheetNinjaBotApiModule(ConfigManager configManager) : base(configManager)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Apis>().As<IApis>();
        }
    }
}
