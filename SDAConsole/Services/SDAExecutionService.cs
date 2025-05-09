using Microsoft.Extensions.Logging;
using SDAConsole.types;
using Pastel;
using Newtonsoft.Json;

namespace InteractiveApp.Services
{

    public static class SDAExecutionService
    {
        private static string outputFileName;
        public static async Task CreateTestObjectFile(List<ILogger> availableLoggers)
        {
            await Program.RunSDAConsole(availableLoggers, true);
            outputFileName = Program.GetOutputFileName();
        }

        public static async Task CreateTestObjectFile(List<ILogger> availableLoggers, List<(string, string)> args = null)
        {
            await Program.RunSDAConsole(availableLoggers, true, args);
            outputFileName = Program.GetOutputFileName();
        }

        public static string GetOutputFileName()
        {
            return outputFileName;
        }

        public static async Task AnalyzeTestObjectFile(SDAConfiguration config, ILogger logger, int mode)
        {
            // Variables
            var files = Array.Empty<string>();
            int totalCompleteNodes = 0;
            int totalIncompleteNodes = 0;

            // Step 1: Ask the user from the available files in the output directory
            // TODO: Create a method for this logic in the DirectoriesHandler class so it can be reused in the BaselineService class and in the "Analyze results" menu option
            string outputDirectory = Path.GetDirectoryName("./output/") ?? string.Empty;
            if (!Directory.Exists(outputDirectory))
            {
                logger.LogError($"Output directory not found: {outputDirectory}");
                return;
            }

            files = Directory.GetFiles(outputDirectory, "*.json");
            if (files.Length == 0)
            {
                logger.LogError("No test object files found in the output directory.");
                return;
            }

            Console.WriteLine("Available test object files:");
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {Path.GetFileName(files[i])}");
            }

            Console.Write("Select a file to analyze (enter the number): ");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > files.Length)
            {
                logger.LogError("Invalid selection.");
                return;
            }

            string path = files[selectedIndex - 1];

            if (!File.Exists(path))
            {
                logger.LogError($"Test object file not found at {path}");
                return;
            }

            try
            {
                string json = await File.ReadAllTextAsync(path);
                var testObj = JsonConvert.DeserializeObject<TestObjectConfiguration>(json);
                string ecuNameInput = string.Empty;
                if (mode == 1)
                {
                    Console.WriteLine("Which ECU do you want to analyze?");
                    // Console.ReadLine();
                    ecuNameInput = Console.ReadLine() ?? string.Empty;

                }
                Console.WriteLine("\nTest Object ECU Summary:\n");

                Console.WriteLine($"{"ECU",-25}{"HW Part Number",-20}{"HW Version",-20}{"CoMo Expression",-20}");
                Console.WriteLine(new string('-', 65));

                foreach (var ecu in testObj.EcuList)
                {
                    string ecuName = ecu.Ecu;

                    // Hardware part number status
                    bool hasHwPart = !string.IsNullOrWhiteSpace(ecu.HardwarePartNumber);
                    string hwStatusRaw = hasHwPart ? "OK" : "Missing";
                    string hwStatus = hwStatusRaw.PadRight(16).Pastel(hasHwPart ? ConsoleColor.Green : ConsoleColor.Red);

                    // Hardware version status
                    bool hasHwVersion = !string.IsNullOrWhiteSpace(ecu.Version);
                    string hwVersionStatusRaw = hasHwVersion ? "OK" : "Missing";
                    string hwVersionStatus = hwVersionStatusRaw.PadRight(16).Pastel(hasHwVersion ? ConsoleColor.Green : ConsoleColor.Red);

                    // CoMo match check
                    string matchKeyPrefix = ecuName + "-";
                    bool hasCoMoMatch = testObj.CoMoExpressions.Any(expr =>
                        !string.IsNullOrWhiteSpace(expr.Comment) && expr.Comment.StartsWith(matchKeyPrefix));

                    string comoStatusRaw = hasCoMoMatch ? "OK" : "Missing";
                    string comoStatus = comoStatusRaw.PadRight(18).Pastel(hasCoMoMatch ? ConsoleColor.Green : ConsoleColor.Red);

                    if (mode == 1 && !ecuName.Contains(ecuNameInput, StringComparison.OrdinalIgnoreCase))
                    {
                        continue; // Skip if the ECU name does not match the input
                    }else if (mode == 1 && ecuName.Contains(ecuNameInput, StringComparison.OrdinalIgnoreCase))
                    {
                        // Print the ECU name, hardware part number status, and CoMo match status
                        Console.WriteLine($"{ecuName.PadRight(25)}{hwStatus}{hwVersionStatus}{comoStatus}");
                        if (hasHwPart && hasCoMoMatch)
                        {
                            totalCompleteNodes++;
                        }
                        else if (!hasHwPart || !hasCoMoMatch)
                        {
                            totalIncompleteNodes++;
                        }

                    }

                    else if (mode == 0)
                    {
                        // Print the ECU name, hardware part number status, and CoMo match status
                        Console.WriteLine($"{ecuName.PadRight(25)}{hwStatus}{hwVersionStatus}{comoStatus}");
                        if (hasHwPart && hasCoMoMatch)
                        {
                            totalCompleteNodes++;
                        }
                        else if (!hasHwPart || !hasCoMoMatch)
                        {
                            totalIncompleteNodes++;
                        }
                    }

                }

                Console.WriteLine(new string('-', 65));
                Console.WriteLine($"Total ECUs: {testObj.EcuList.Count}");
                Console.WriteLine($"Complete Nodes: {totalCompleteNodes}");
                Console.WriteLine($"Incomplete Nodes: {totalIncompleteNodes}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to analyze test object file.");
                Console.WriteLine("Error analyzing test object file. Check logs for more info.");
            }
        }
    }
}
