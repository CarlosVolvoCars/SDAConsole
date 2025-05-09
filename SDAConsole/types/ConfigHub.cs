using SDAConsole.interfaces.ConfigHub;

namespace SDAConsole.types.ConfigHub
{
    public class ConfigHubConfiguration : IConfigHubConfig
    {
        public string BaseUrl_Prod { get; set; } = string.Empty;
        public string SessionUrl { get; set; } = string.Empty;
        public string GetBaselineByNameUrl { get; set; } = string.Empty;
        public string GetCompatibleHardwareFromBaselineByIDUrl { get; set; } = string.Empty;
        public string GetCompatibleSoftwareFromBaselineByIDUrl { get; set; } = string.Empty;
        public int TimeoutInSecs { get; set; } = 30;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TargetBaselineName { get; set; } = string.Empty;
        public string TargetBaselineID { get; set; } = string.Empty;
        public ConfigHubBaseline BaselineMetaData { get; set; } = new ConfigHubBaseline();

        public void SetBaselineMetaData(ConfigHubBaseline baseLines)
        {
            BaselineMetaData = baseLines;
        }

        public void SetTargetBaselineID(string baseLine)
        {
            TargetBaselineID = baseLine;
        }

    }

    public enum ServerInstanceOption
    {
        PROD,
        QA,
        DEV
    }

    public class ConfigHubException : Exception
    {
        public ConfigHubException(string message) : base(message) { }
        public ConfigHubException(string message, Exception inner) : base(message, inner) { }
    }

    public class ConfigHubBaseline
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Handle { get; set; } = string.Empty;
        public string IntentOfUse { get; set; } = string.Empty;
        public string SubIntentOfUse { get; set; } = string.Empty;
        // public List<ConfigHubPostVbf> PostVbfs { get; set; } = new List<ConfigHubPostVbf>();
        // public List<ConfigHubPostVbf> BlockedPostVbfs { get; set; } = new List<ConfigHubPostVbf>();
        public List<ConfigHubSubBaseline> SubBaselines { get; set; } = new List<ConfigHubSubBaseline>();
        // public List<ConfigHubVariant> Variants { get; set; } = new List<ConfigHubVariant>();
        public string ComponentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PositionStructure { get; set; } = string.Empty;
        // public ConfigHubPosition Pos { get; set; } = new ConfigHubPosition();
    }

    public class ConfigHubSubBaseline
    {
        public string Name { get; set; } = string.Empty;
        public string Handle { get; set; } = string.Empty;
        public string IntentOfUse { get; set; } = string.Empty;
        public string SubIntentOfUse { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        // public List<ConfigHubVariant> Variants { get; set; } = new List<ConfigHubVariant>();
        public List<string> SubBaselines { get; set; } = new List<string>();
        // public ConfigHubEvent Event { get; set; } = new ConfigHubEvent();
        // public ConfigHubPosition Pos { get; set; } = new ConfigHubPosition();
    }


}
