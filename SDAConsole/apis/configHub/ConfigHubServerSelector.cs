using SDAConsole.interfaces.API;
using SDAConsole.types.ConfigHub;
using SDAConsole.interfaces.ConfigHub;

namespace SDAConsole.ConfigHub
{
    public class ConfigHubServerSelector : IApiServerSelector
    {
        private readonly IConfigHubConfig config;

        public ConfigHubServerSelector(IConfigHubConfig config)
        {
            this.config = config;
        }

        public string GetBaseUrl(ServerInstanceOption instance)
        {
            return instance switch
            {
                ServerInstanceOption.PROD => config.BaseUrl_Prod,
                _ => throw new ConfigHubException($"Unsupported instance: {instance}")
            };
        }
    }
}