using Autofac;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public class Bootstrapper
    {
        public static Bootstrapper Initialize(Action<ConfigManager, ContainerBuilder> update, IDictionary<string, string> dynamicSettings)
        {
            var builder = new ContainerBuilder();

            var mergedDynamicSettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // TODO: Should read from a config file or be parameterized for this cmdlet

                { "environment" , "develop"},
            };
            foreach (var dynamicSetting in dynamicSettings)
            {
                mergedDynamicSettings[dynamicSetting.Key] = dynamicSetting.Value;
            }

            ConfigManager configManager = new ConfigManager(dynamicSettings: mergedDynamicSettings);

            update(configManager, builder);
            var bootstrapper = new Bootstrapper(builder);
            return bootstrapper;
        }

        public IContainer Container { get; private set; }

        public Bootstrapper(ContainerBuilder builder)
        {
            this.Container = builder.Build();
        }
    }
}
