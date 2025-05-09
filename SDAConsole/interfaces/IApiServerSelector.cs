using SDAConsole.types.ConfigHub;

namespace SDAConsole.interfaces.API
{
    public interface IApiServerSelector
    {
        string GetBaseUrl(ServerInstanceOption instance);
    }
}