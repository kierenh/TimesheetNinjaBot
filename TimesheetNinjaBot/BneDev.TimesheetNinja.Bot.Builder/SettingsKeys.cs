using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BneDev.TimesheetNinja.Common;

namespace BneDev.TimesheetNinja.Bot.Builder
{
    public static class SettingsKeys
    {
        public const string AppDataPath = "AppDataPath";

        public const string IsDemoMode = "Demo.IsDemoMode";

        public const string UseLuis = "LUIS_IsEnabled";

        public const string LuisModelId = "LUIS_ModelId";

        public const string LuisSubscriptionKey = "LUIS_SubscriptionKey";

        public const string LuisApiVersion = "LUIS_ApiVersion";

        public const string LuisDomain = "LUIS_Domain";

        public const string UseCosmosDbForConversationState = "UseCosmosDbForConversationState";

        public const string DocumentDbServiceEndpoint = "DocumentDbServiceEndpoint";

        public const string DocumentDbAuthKey = "DocumentDbAuthKey";

        public const string UseTableStorageForConversationState = "UseTableStorageForConversationState";

        public const string StorageConnectionString = "BotStateStorage";

    }
}
