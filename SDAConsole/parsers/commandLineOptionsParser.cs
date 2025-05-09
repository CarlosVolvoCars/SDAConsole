using SDAConsole.types.CommandLineOptions;

public static class SDAArgsParser
{
    public static SDACommandLineOptions Parse(string[] args)
    {
        var options = new SDACommandLineOptions();

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            string? next = (i + 1 < args.Length) ? args[i + 1] : null;

            switch (arg)
            {
                case "--config": options.ConfigPath = next; break;
                case "--testObjectTemplatePath": options.TestObjectTemplatePath = next; break;
                case "--baseCarConfigPath": options.BaseCarConfigPath = next; break;
                case "--baseInstructionPath": options.BaseInstructionPath = next; break;
                case "--baseInstructionReportPath": options.BaseInstructionReportPath = next; break;
                case "--testObjectOutputPath": options.TestObjectOutputPath = next; break;
                case "--ecuListFilePath": options.EcuListFilePath = next; break;
                case "--vehicleTypeCodesFilePath": options.VehicleTypeCodesFilePath = next; break;
                case "--vehicleFirstReadoutFilePath": options.VehicleFirstReadoutFilePath = next; break;
                case "--configHubSettingsPath": options.ConfigHubSettingsPath = next; break;
                case "--excludedEcusFilePath": options.ExcludedEcusFilePath = next; break;
                case "--interactiveMode": options.InteractiveMode = true; break;
                case "--version": options.ShowVersion = true; break;
                case "--help": options.ShowHelp = true; break;
            }
        }

        return options;
    }
}
