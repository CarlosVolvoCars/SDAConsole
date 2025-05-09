using SDAConsole.types.ConfigHub;

namespace InteractiveApp.Models
{
    public class AppConfig
    {
        public string dalAppSettingsPath { get; set; }
        public string testObjectTemplateConfigPath { get; set; }
        public string baseCarConfigPath { get; set; }
        public string baseInstructionPath { get; set; }
        public string baseInstructionReportPath { get; set; }
        public string testObjectOutputFilePath { get; set; }
        public string ecuListFilePath { get; set; }
        public string vehicleTypeCodesFilePath { get; set; }
        public string vehicleFirstReadoutFilePath { get; set; }
        public string confighubSettingsPath { get; set; }
        public string excludedECUListFilePath { get; set; }
        public string outputFolderPath { get; set; }
        public string exportedExcelFilePath { get; set; } = string.Empty;
        public List<string> steeringCoMoExpressions { get; set; } = new();
        public List<HardwarePartInteractiveApp> forcedHardwarePartNumbers { get; set; } = new();
        public List<forcedCoMoExpression> forcedCoMoExpressions { get; set; } = new List<forcedCoMoExpression>();

        public ConfigHubConfiguration configHubConfiguration = new ConfigHubConfiguration();
    }


    public class HardwarePartInteractiveApp
    {
        public string EcuName { get; set; }
        public string EcuAddress { get; set; }
        public string EcuHardwarePartNumber { get; set; }
        public string EcuHardwarePartVersion { get; set; }
    }

    public class forcedCoMoExpression
    {
        public string Comment { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string ecuSolutionParameter { get; set; } = string.Empty;
        public List<string> ConfigurationParameterValues { get; set; } = new List<string>();
    }

}
