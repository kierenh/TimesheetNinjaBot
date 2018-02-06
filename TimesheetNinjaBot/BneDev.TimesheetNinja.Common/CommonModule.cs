using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace BneDev.TimesheetNinja.Common
{
    public class CommonModule : ModuleBase
    {
        public CommonModule(ConfigManager configManager) : base(configManager)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => this.ConfigManager);
            builder.RegisterType<DebugLogger>().As<ILogger>();
            var systemDateTime = ConfigManager.GetAppSetting(SettingsKeys.SystemDateTime);

            var registration = builder.RegisterType<DateTimeService>().As<IDateTimeService>();
            if (!String.IsNullOrWhiteSpace(systemDateTime))
            {
                registration.WithProperties(new[]
                {
                    new NamedPropertyParameter(SettingsKeys.SystemDateTime, DateTimeService.ParseIso8601Date(systemDateTime))
                });
            }

            builder.RegisterType<CommonServices>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
