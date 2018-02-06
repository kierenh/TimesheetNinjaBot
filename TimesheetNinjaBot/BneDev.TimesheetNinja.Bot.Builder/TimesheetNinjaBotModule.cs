using Autofac;
using Autofac.Core;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using BneDev.TimesheetNinja.Common;
using BneDev.TimesheetNinja.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac.Integration.WebApi;
using System.Diagnostics;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinja.Bot.Api;
using System.Globalization;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    /// <summary>
    /// The Timesheet Ninja Bot Module.
    /// </summary>
    public class TimesheetNinjaBotModule : ModuleBase
    {
        public TimesheetNinjaBotModule(ConfigManager configManager, HttpConfiguration httpConfiguration)
            : base(configManager)
        {
            // todo: can we map app_data without System.Web.dll :/
            configManager.AddDynamicAppSetting(SettingsKeys.AppDataPath, System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/"));
            this.HttpConfiguration = httpConfiguration;
        }

        protected HttpConfiguration HttpConfiguration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterConversationStateStorage(builder);
            RegisterLuisServices(builder);
            RegisterDialogs(builder);
        }

        private void RegisterConversationStateStorage(ContainerBuilder builder)
        {
            // The default Connector State Service used by bots is not intended for the production environment. 
            // Using Azure Extensions available on GitHub

            IBotDataStore<BotData> store = null;

            bool.TryParse(ConfigManager.GetAppSetting(SettingsKeys.UseCosmosDbForConversationState), out bool useCosmosDbForConversationState);
            if (useCosmosDbForConversationState)
            {
                Uri docDbServiceEndpoint = new Uri(ConfigManager.GetAppSetting(SettingsKeys.DocumentDbServiceEndpoint));
                string docDbEmulatorKey = ConfigManager.GetAppSetting(SettingsKeys.DocumentDbAuthKey);

                store = new DocumentDbBotDataStore(docDbServiceEndpoint, docDbEmulatorKey);
            }
            else
            {
                bool.TryParse(ConfigManager.GetAppSetting(SettingsKeys.UseTableStorageForConversationState), out bool useTableStorageForConversationState);
                if (useTableStorageForConversationState)
                {
                    store = new TableBotDataStore(ConfigManager.ConnectionStrings[SettingsKeys.StorageConnectionString].ConnectionString);
                }
            }

            if (store != null)
            {
                builder.Register(c => store)
                       .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                       .AsSelf()
                       .SingleInstance();

                builder.Register(c => new CachingBotDataStore(store, CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                       .As<IBotDataStore<BotData>>()
                       .AsSelf()
                       .InstancePerLifetimeScope();
            }
        }

        protected virtual void RegisterLuisServices(ContainerBuilder builder)
        {
            // Register and setup LUIS service
            var modelId = ConfigManager.GetAppSetting(SettingsKeys.LuisModelId);
            var subscriptionKey = ConfigManager.GetAppSetting(SettingsKeys.LuisSubscriptionKey);
            Enum.TryParse(ConfigManager.GetAppSetting(SettingsKeys.LuisApiVersion), out LuisApiVersion luisApiVersion);
            var domain = ConfigManager.GetAppSetting(SettingsKeys.LuisDomain);

            builder.Register<IEnumerable<ILuisService>>(context => new[]
            {
                new LuisService(new LuisModelAttribute(modelId, subscriptionKey, luisApiVersion, domain))
            });
        }

        protected virtual void RegisterDialogs(ContainerBuilder builder)
        {
            builder.RegisterType<DialogServices>().AsSelf().InstancePerLifetimeScope();

            var isDemoMode = Convert.ToBoolean(ConfigManager.GetAppSetting(SettingsKeys.IsDemoMode), CultureInfo.InvariantCulture);
            builder.RegisterType<GetProjectsDialog>().AsSelf();
            builder.RegisterType<AddExpenseDialog>().AsSelf();
            builder.RegisterType<GetExpensesDialog>().AsSelf();
            builder.RegisterType<AddTimeDialog>()
                .WithProperty(new NamedPropertyParameter(nameof(AddTimeDialog.IsDemoMode), isDemoMode))
                .AsSelf();
            builder.RegisterType<CheckSubmissionStatusDialog>().AsSelf();
            builder.RegisterType<SetSessionDialog>().AsSelf();
            builder.RegisterType<GetTimeDialog>().AsSelf();
            builder.RegisterType<RootDialog>()
                .Named<IDialog<object>>(nameof(RootDialog))
                .WithProperties(new[] {
                    new NamedPropertyParameter(nameof(RootDialog.UseLuis), Convert.ToBoolean(ConfigManager.GetAppSetting(SettingsKeys.UseLuis), CultureInfo.InvariantCulture)),
                    new NamedPropertyParameter(nameof(RootDialog.IsDemoMode), isDemoMode)
                });
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
                        new TimesheetNinjaBotApiModule(ConfigManager),
                        new CommonModule(ConfigManager),
                        new AzureModule(Assembly.GetExecutingAssembly()),
                        new ReflectionSurrogateModule(),
                        new GlobalMessageHandlersBotModule(),
                    };
                }
                return dependencies;
            }
        }
    }
}
