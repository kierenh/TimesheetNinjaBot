using Autofac;
using Autofac.Core;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BneDev.TimesheetNinja.Common
{
    /// <summary>
    /// A module is a small class that can be used to bundle up a set of related components behind a ‘facade’ to simplify configuration and deployment. 
    /// The module exposes a deliberate, restricted set of configuration parameters that can vary independently of the components used to implement the module.
    /// 
    /// This extends <see cref="Autofac"/> by capturing module dependencies and making <see cref="Configuration"/> easily accessible and available to all modules in the dependency chain.
    /// </summary>
    public abstract class ModuleBase : Module
    {
        /// <summary>
        /// Gets or sets the <see cref="ConfigManager"/>.
        /// </summary>
        /// <remarks>Strictly speaking this _shouldn't_ really be registered in the container - because we  want components to be configured from within modules, which have access to this <see cref="ModuleBase.ConfigManager"/> property.
        /// But, is here for completeness, in case there is the odd setting you want to look-up via the <see cref="ConfigManager"/> directly.
        /// </remarks>
        public ConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ModuleBase(ConfigManager configManager)
        {
            this.ConfigManager = configManager;
        }

        /// <summary>
        /// Gets the modules for which this module is dependent.
        /// </summary>
        /// <remarks>
        /// This is a simple way to decouple loading of dependent modules without having to add all references 
        /// to the top-level calling solution component and without too much magic for finding/searching assemblies for modules to load (i.e. because your dependencies are explicit).
        /// </remarks>
        public virtual IList<IModule> Dependencies => new IModule[0];

        protected override void Load(ContainerBuilder builder)
        {
            if (this.Dependencies.Any())
            {
                foreach (var module in this.Dependencies)
                {
                    var moduleBase = module as ModuleBase;
                    if (moduleBase != null)
                    {
                        moduleBase.ConfigManager = ConfigManager;
                    }
                    builder.RegisterModule(module);
                }
            }
            base.Load(builder);
        }

        public bool IsDevelop => String.Equals(ConfigManager.GetAppSetting(SettingsKeys.Environment), "develop", StringComparison.OrdinalIgnoreCase);

        public bool IsProduction => String.Equals(ConfigManager.GetAppSetting(SettingsKeys.Environment), "production", StringComparison.OrdinalIgnoreCase);

    }
}
