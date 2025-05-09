namespace SDAConsole.types
{
    public class SDAConfiguration
    {
        public string dalAppSettingsPath { get; set; } = string.Empty;
        public string testObjectTemplateConfigPath { get; set; } = string.Empty;
        public string baseCarConfigPath { get; set; } = string.Empty;
        public string baseInstructionPath { get; set; } = string.Empty;
        public string baseInstructionReportPath { get; set; } = string.Empty;
        public string testObjectOutputFilePath { get; set; } = string.Empty;
        public string ecuListFilePath { get; set; } = string.Empty;
        public string vehicleTypeCodesFilePath { get; set; } = string.Empty;
        public string vehicleFirstReadoutFilePath { get; set; } = string.Empty;
        public string confighubSettingsPath { get; set; } = string.Empty;
        public string excludedECUListFilePath { get; set; } = string.Empty;
        public string outputFolderPath { get; set; } = string.Empty;
        public string exportedExcelFilePath { get; set; } = string.Empty;
        public List<string> steeringCoMoExpressions { get; set; } = new List<string>();
        public List<forcedHardwarePartNumber> forcedHardwarePartNumbers { get; set; } = new List<forcedHardwarePartNumber>();
        public List<forcedCoMoExpression> forcedCoMoExpressions { get; set; } = new List<forcedCoMoExpression>();
    }

    public class forcedHardwarePartNumber
    {
        public string EcuName { get; set; } = string.Empty;
        public string EcuAddress { get; set; } = string.Empty;
        public string EcuHardwarePartNumber { get; set; } = string.Empty;
        public string EcuHardwarePartVersion { get; set; } = string.Empty;
    }

    public class forcedCoMoExpression
    {
        public string Comment { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string ecuSolutionParameter { get; set; } = string.Empty;
        public List<string> ConfigurationParameterValues { get; set; } = new List<string>();
    }

}