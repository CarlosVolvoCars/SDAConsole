namespace SDAConsole.types.CommandLineOptions
{
    public class SDACommandLineOptions
    {
        public string? ConfigPath { get; set; }
        public string? TestObjectTemplatePath { get; set; }
        public string? BaseCarConfigPath { get; set; }
        public string? BaseInstructionPath { get; set; }
        public string? BaseInstructionReportPath { get; set; }
        public string? TestObjectOutputPath { get; set; }
        public string? EcuListFilePath { get; set; }
        public string? VehicleTypeCodesFilePath { get; set; }
        public string? VehicleFirstReadoutFilePath { get; set; }
        public string? ConfigHubSettingsPath { get; set; }
        public string? ExcludedEcusFilePath { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowVersion { get; set; }
        public bool InteractiveMode { get; set; }
    }
}