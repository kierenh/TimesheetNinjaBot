using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public class ConfigManager
    {
        private IDictionary<string, string> dynamicSettings;

        public ConfigManager(Configuration configuration = null, IDictionary<string, string> dynamicSettings = null)
        {
            this.Configuration = configuration;
            this.dynamicSettings = dynamicSettings ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets the connection strings.
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return Configuration.ConnectionStrings.ConnectionStrings; }
        }

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        protected KeyValueConfigurationCollection AppSettings
        {
            get { return Configuration.AppSettings.Settings; }
        }

        /// <summary>
        /// Get value corresponding to the key from application settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetAppSetting(string key)
        {
            // Note: Resolving via Environment would be more Docker Container friendly
            // return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            // but, until then...
            if (!dynamicSettings.TryGetValue(key, out string value))
            {
                return AppSettings[key].Value;
            }
            return value;
        }

        public void AddDynamicAppSetting(string key, string value)
        {
            dynamicSettings.Add(key, value);
        }
    }
}
