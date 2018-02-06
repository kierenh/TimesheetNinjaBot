using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Dialogs;
using BneDev.TimesheetNinja.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Routing;
using BneDev.TimesheetNinja.Bot.Builder;
using BneDev.TimesheetNinjaBot.BffServices;
using BneDev.TimesheetNinja.Bot.Api;
using Autofac.Core;
using BneDev.TimesheetNinja.Bot;

namespace BneDev.TimesheetNinjaBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            ConfigManager configManager = new ConfigManager(configuration);
            Conversation.UpdateContainer(builder => builder.RegisterModule(new TimesheetNinjaBotDemoModule(configManager, GlobalConfiguration.Configuration)));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);
        }
    }
}
