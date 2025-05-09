using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using InteractiveApp.Models;
using Pastel;
using SDAConsole.types;
using SDAConsole.loaders;


namespace InteractiveApp.Config
{
    public class ConfigManager()
    {
        private const string configPath = "./configs/configuration.json";
        private static AppConfig config = LoadConfig(configPath);
        private static AppConfig LoadConfig(string configPath)
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<AppConfig>(json)!;
        }

        public void ReloadConfig()
        {
            config = LoadConfig(configPath);
            Console.WriteLine("Configuration reloaded.");
        }

        public void SaveConfig(string configTarget)
        {
            var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

            switch (configTarget)
            {
                case "configuration":
                    string json = JsonSerializer.Serialize(config, options);
                    File.WriteAllText(configPath, json);

                    Program.sdaConfiguration = SDAConfigurationLoader.LoadFromFile(configPath);
                    Console.WriteLine("Configuration reloaded.");

                    break;
                case "configurationConfigHub":
                    // serialize the configHubConfiguration but not its BaselineMetaData property
                    config.configHubConfiguration.BaselineMetaData = null;

                    string jsonConfigHub = JsonSerializer.Serialize(config.configHubConfiguration, options);

                    File.WriteAllText(config.confighubSettingsPath, jsonConfigHub);
                    break;
                default:
                    throw new ArgumentException("Invalid config target specified.");
            }

        }

        public void ListSteeringCoMo()
        {
            int index = 0;
            Console.Clear();
            Console.WriteLine("Steering CoMo Expressions:");
            foreach (var expr in config.steeringCoMoExpressions)
                {
                    Console.WriteLine($"{index}- {expr}");
                    index++;
                }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void EditSteeringCoMo()
        {
            ListSteeringCoMo();
            Console.Write("Enter the index of the expression to edit: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < config.steeringCoMoExpressions.Count)
            {
                Console.Write("Enter new CoMo Expression: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    config.steeringCoMoExpressions[index] = input;
                    SaveConfig("configuration");
                    Console.WriteLine("Updated.");
                }
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void DeleteSteeringCoMo()
        {
            ListSteeringCoMo();
            Console.Write("Enter the index of the expression to delete: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < config.steeringCoMoExpressions.Count)
            {
                config.steeringCoMoExpressions.RemoveAt(index);
                SaveConfig("configuration");
                Console.WriteLine("Deleted.");
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void AddSteeringCoMo()
        {
            Console.Write("Enter new CoMo Expression: ");
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                config.steeringCoMoExpressions.Add(input);
                SaveConfig("configuration");
                Console.WriteLine("Added.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ListHardwareParts()
        {
            int index = 0;
            Console.Clear();
            Console.WriteLine("Forced Hardware Part Numbers:");
            foreach (var part in config.forcedHardwarePartNumbers)
            {
                Console.WriteLine($"{index}- {part.EcuName} > {part.EcuAddress} > {part.EcuHardwarePartNumber} > {part.EcuHardwarePartVersion}");
                index++;
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void EditHardwareParts()
        {
            ListHardwareParts();
            Console.Write("Enter the index of the hardware part to edit: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < config.forcedHardwarePartNumbers.Count)
            {
                var part = config.forcedHardwarePartNumbers[index];
                Console.Write($"Enter new EcuName (current: {part.EcuName}): ");
                var ecuName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ecuName))
                    part.EcuName = ecuName;

                Console.Write($"Enter new EcuAddress (current: {part.EcuAddress}): ");
                var ecuAddress = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ecuAddress))
                    part.EcuAddress = ecuAddress;

                Console.Write($"Enter new EcuHardwarePartNumber (current: {part.EcuHardwarePartNumber}): ");
                var ecuHardwarePartNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ecuHardwarePartNumber))
                    part.EcuHardwarePartNumber = ecuHardwarePartNumber;

                Console.Write($"Enter new EcuHardwarePartVersion (current: {part.EcuHardwarePartVersion}): ");
                var ecuHardwarePartVersion = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(ecuHardwarePartVersion))
                    part.EcuHardwarePartVersion = ecuHardwarePartVersion;

                SaveConfig("configuration");
                Console.WriteLine("Updated.");
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void DeleteHardwareParts()
        {
            ListHardwareParts();
            Console.Write("Enter the index of the hardware part to delete: ");
            if (int.TryParse(Console.ReadLine(), out int index)
                && index >= 0 && index < config.forcedHardwarePartNumbers.Count)
            {
                config.forcedHardwarePartNumbers.RemoveAt(index);
                SaveConfig("configuration");
                Console.WriteLine("Deleted.");
            }
            else
            {
                Console.WriteLine("Invalid index.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

        }

        public void AddHardwareParts()
        {
            var part = new HardwarePartInteractiveApp();
            Console.Write("Enter EcuName: ");
            part.EcuName = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter EcuAddress: ");
            part.EcuAddress = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter EcuHardwarePartNumber: ");
            part.EcuHardwarePartNumber = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter EcuHardwarePartVersion: ");
            part.EcuHardwarePartVersion = Console.ReadLine() ?? string.Empty;

            config.forcedHardwarePartNumbers.Add(part);
            SaveConfig("configuration");
            Console.WriteLine("Added.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ListSDASettings()
        {
            string tempBufferPropertyName;
            string propertyNameFormatted;

            Console.Clear();
            Console.WriteLine("SDA Settings:");
            Console.WriteLine("-------------------------------------------------");
            tempBufferPropertyName = "DAL App Settings Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.dalAppSettingsPath}");

            tempBufferPropertyName = "Test Object Template Config Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.testObjectTemplateConfigPath}");

            tempBufferPropertyName = "Base Car Config Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.baseCarConfigPath}");

            tempBufferPropertyName = "Base Instruction Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.baseInstructionPath}");

            tempBufferPropertyName = "Base Instruction Report Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.baseInstructionReportPath}");

            tempBufferPropertyName = "Test Object Output File Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.testObjectOutputFilePath}");

            tempBufferPropertyName = "ECU List File Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.ecuListFilePath}");

            tempBufferPropertyName = "Vehicle Type Codes File Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.vehicleTypeCodesFilePath}");

            tempBufferPropertyName = "Vehicle First Readout File Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.vehicleFirstReadoutFilePath}");

            tempBufferPropertyName = "Confighub Settings Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.confighubSettingsPath}");

            tempBufferPropertyName = "Excluded ECU List File Path:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.excludedECUListFilePath}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void EditSDASettings()
        {
            ListSDASettings();
            Console.Write("Enter the setting to edit (e.g., dalAppSettingsPath): ");
            var setting = Console.ReadLine();
            if (setting != null && config.GetType().GetProperty(setting) != null)
            {
                Console.Write($"Enter new value for {setting}: ");
                var value = Console.ReadLine();
                config.GetType().GetProperty(setting)?.SetValue(config, value);
                SaveConfig("configuration");
                Console.WriteLine("Updated.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

        }

        public void DeleteSDASettings()
        {
            ListSDASettings();
            Console.Write("Enter the setting to delete (e.g., dalAppSettingsPath): ");
            var setting = Console.ReadLine();
            if (setting != null && config.GetType().GetProperty(setting) != null)
            {
                config.GetType().GetProperty(setting)?.SetValue(config, string.Empty);
                SaveConfig("configuration");
                Console.WriteLine("Deleted.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void AddSDASettings()
        {
            Console.Write("Enter new DAL setting (e.g., dalAppSettingsPath): ");
            var setting = Console.ReadLine();
            if (setting != null && config.GetType().GetProperty(setting) != null)
            {
                Console.Write($"Enter value for {setting}: ");
                var value = Console.ReadLine();
                config.GetType().GetProperty(setting)?.SetValue(config, value);
                SaveConfig("configuration");
                Console.WriteLine("Added.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void ListConfighubSettings()
        {
            string tempBufferPropertyName;
            string propertyNameFormatted;

            config.configHubConfiguration = ConfigHubConfigurationLoader.LoadFromFile(config.confighubSettingsPath);

            Console.Clear();
            Console.WriteLine("Confighub Settings:");
            Console.WriteLine("-------------------------------------------------");
            tempBufferPropertyName = "0 - Timeout In Secs:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.configHubConfiguration.TimeoutInSecs}");
            tempBufferPropertyName = "1 - User Name:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.configHubConfiguration.UserName}");
            tempBufferPropertyName = "2 - Password:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.configHubConfiguration.Password}");
            tempBufferPropertyName = "3 - Target Baseline Name:";
            propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            Console.WriteLine($"{propertyNameFormatted} {config.configHubConfiguration.TargetBaselineName}");
            // TODO: Provide a way to set the TargetBaselineID in the config file so the Baseline can be fetch with the ID before enabling the config
            // tempBufferPropertyName = "4 - Target Baseline ID:";
            // propertyNameFormatted = tempBufferPropertyName.PadRight(45).Pastel(ConsoleColor.Green);
            // Console.WriteLine($"{propertyNameFormatted} {config.configHubConfiguration.TargetBaselineID}");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void EditConfighubSettings()
        {
            ListConfighubSettings();
            Console.Write("Enter the setting to edit (e.g., 0 for 'Timeout In Secs'): ");
            var setting = Console.ReadLine();

            int.TryParse(setting, out int index);
            index = index + 5; // Adjust index to match editable properties
            if ( index >= 0 && index < 9 && index < config.configHubConfiguration.GetType().GetProperties().Length)
            {
                var property = config.configHubConfiguration.GetType().GetProperties()[index];
                Console.Write($"Enter new value for {property.Name}: ");
                var stringValue = Console.ReadLine();

                // Check if the property type is int and convert it accordingly
                if (property.PropertyType == typeof(int) && int.TryParse(stringValue, out int intValue))
                {

                    property.SetValue(config.configHubConfiguration, intValue);
                }
                else if (property.PropertyType == typeof(bool) && bool.TryParse(stringValue, out bool boolValue))
                {

                    property.SetValue(config.configHubConfiguration, boolValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    // No conversion needed for string
                    property.SetValue(config.configHubConfiguration, stringValue);
                }
                else
                {
                    Console.WriteLine("Invalid value type.");
                    return;
                }

                SaveConfig("configurationConfigHub");
                Console.WriteLine("Updated.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void DeleteConfighubSettings()
        {
            ListConfighubSettings();
            Console.Write("Enter the setting to delete (e.g., confighubSettingsPath): ");
            var setting = Console.ReadLine();
            if (setting != null && config.GetType().GetProperty(setting) != null)
            {
                config.GetType().GetProperty(setting)?.SetValue(config, string.Empty);
                SaveConfig("configurationConfigHub");
                Console.WriteLine("Deleted.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void AddConfighubSettings()
        {
            Console.Write("Enter new Confighub setting (e.g., confighubSettingsPath): ");
            var setting = Console.ReadLine();
            if (setting != null && config.GetType().GetProperty(setting) != null)
            {
                Console.Write($"Enter value for {setting}: ");
                var value = Console.ReadLine();
                config.GetType().GetProperty(setting)?.SetValue(config, value);
                SaveConfig("configurationConfigHub");
                Console.WriteLine("Added.");
            }
            else
            {
                Console.WriteLine("Invalid setting.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    }
}
