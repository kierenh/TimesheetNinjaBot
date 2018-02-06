using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Common;
using BneDev.TimesheetNinjaBot.BffServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BneDev.TimesheetNinja.Bot
{
    public class TimesheetNinjaBotDemoModule : ModuleBase
    {
        private HttpConfiguration httpConfiguration;

        public TimesheetNinjaBotDemoModule(ConfigManager configManager, HttpConfiguration httpConfiguration)
            : base(configManager)
        {
            this.httpConfiguration = httpConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Read required app settings for demo
            var dateTimeMin = DateTimeService.ParseIso8601Date(ConfigManager.GetAppSetting("demo.DateTimeMin"));
            var dateTimeMax = DateTimeService.ParseIso8601Date(ConfigManager.GetAppSetting("demo.DateTimeMax"));

            builder.RegisterType<TimesheetNinjaBotMockBffService>()
                .WithParameters(new[]
                {
                    new NamedParameter("appDataPath", ConfigManager.GetAppSetting( Builder.SettingsKeys.AppDataPath ))
                })
                .WithProperties(new[]
                {
                    new NamedPropertyParameter(nameof(TimesheetNinjaBotMockBffService.DateTimeMin), dateTimeMin ),
                    new NamedPropertyParameter(nameof(TimesheetNinjaBotMockBffService.DateTimeMax), dateTimeMax )
                })
                .AsSelf()
                .AsImplementedInterfaces();

            // Register your Web API controllers.
            RegisterWebApiControllers(builder);
        }

        private void RegisterWebApiControllers(ContainerBuilder builder)
        {
            // Register your Web API controllers.
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());
            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(this.httpConfiguration);
            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();
        }

        private IList<IModule> dependencies;

        public override IList<IModule> Dependencies
        {
            get
            {
                if (dependencies == null)
                {
                    dependencies = new IModule[]
                    {
                        new TimesheetNinjaBotModule(ConfigManager, httpConfiguration)
                    };
                }
                return dependencies;
            }
        }
    }
}
