using System;
using System.Collections.Generic;
using InteractiveApp.Config;
using InteractiveApp.Services;
using Microsoft.Extensions.Logging;
using SDAConsole.types;

namespace InteractiveApp.InteractiveMenu
{
    public class MenuContext
    {
        private Stack<MenuItem> menuStack = new();
        private int selectedIndex = 0;
        private readonly ILogger _logger;
        private readonly List<ILogger> _availableLoggers;

        private readonly ConfigManager _configManager;

        private string compareBaselineFile1 = string.Empty;
        private string compareBaselineFile2 = string.Empty;

        public MenuContext( List<ILogger> availableLoggers)
        {

            _availableLoggers = availableLoggers;
            _logger = _availableLoggers[0];

            _configManager = new ConfigManager();

            menuStack.Push(BuildRootMenu());
        }

        public void DisplayCurrentMenu()
        {
            var current = menuStack.Peek();
            Console.WriteLine(current.Title + "\n");

            for (int i = 0; i < current.Children.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {current.Children[i].Title}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {current.Children[i].Title}");
                }
            }
        }

        public void MoveUp() => selectedIndex = (selectedIndex - 1 + Current.Children.Count) % Current.Children.Count;
        public void MoveDown() => selectedIndex = (selectedIndex + 1) % Current.Children.Count;
        public async Task Select()
        {
            var selected = Current.Children[selectedIndex];
            if (selected.Children.Count > 0)
            {
                menuStack.Push(selected);
                selectedIndex = 0;
            }
            else
            {
                if (selected.Action != null)
                    await selected.Action();
            }
        }


        public void GoBack()
        {
            if (menuStack.Count > 1)
            {
                menuStack.Pop();
                selectedIndex = 0;
            }
        }

        private MenuItem Current => menuStack.Peek();

        private MenuItem BuildRootMenu()
        {
            return new MenuItem("Main Menu", new List<MenuItem>
            {
                new("Tool Configuration", new List<MenuItem>
                {
                    new("Steering CoMo Expressions", new List<MenuItem>
                    {
                        new("List", async () => await Task.Run(() => _configManager.ListSteeringCoMo())),
                        new("Edit", async () => await Task.Run(() => _configManager.EditSteeringCoMo())),
                        new("Delete", async () => await Task.Run(() => _configManager.DeleteSteeringCoMo())),
                        new("Add", async () => await Task.Run(() => _configManager.AddSteeringCoMo()))
                    }),
                    new("Forced Hardware Part Numbers", new List<MenuItem>
                    {
                        new("List", async () => await Task.Run(() => _configManager.ListHardwareParts())),
                        new("Edit", async () => await Task.Run(() => _configManager.EditHardwareParts())),
                        new("Delete", async () => await Task.Run(() => _configManager.DeleteHardwareParts())),
                        new("Add", async () => await Task.Run(() => _configManager.AddHardwareParts()))
                    }),
                    new("SDA Settings", new List<MenuItem>
                    {
                        new("List", async () => await Task.Run(() => _configManager.ListSDASettings())),
                        // new("Edit", async () => await Task.Run(() => _configManager.EditSDASettings())),
                        // new("Delete", async () => await Task.Run(() => _configManager.DeleteSDASettings())),
                        // new("Add", async () => await Task.Run(() => _configManager.AddSDASettings()))
                    }),
                    new("Confighub Settings", new List<MenuItem>
                    {
                        new("List", async () => await Task.Run(() => _configManager.ListConfighubSettings())),
                        new("Edit", async () => await Task.Run(() => _configManager.EditConfighubSettings())),
                        // new("Delete", async () => await Task.Run(() => _configManager.DeleteConfighubSettings())),
                        // new("Add", async () => await Task.Run(() => _configManager.AddConfighubSettings()))
                    })
                }),
                new("Restore Tool Configuration", async () => await Task.Run(() => RestoreService.RestoreBackup(_configManager))),
                new("Create Test Object File", async () =>
                    {
                        Console.WriteLine("Creating test object file...");
                        await SDAExecutionService.CreateTestObjectFile(this._availableLoggers);
                        Console.WriteLine("Test object creation completed. Press any key to continue...");
                        Console.ReadKey();
                    }),
                new("Analyze Test Object File", new List<MenuItem>
                {
                    new("Full report", async () =>
                    {
                        int mode = 0;
                        await SDAExecutionService.AnalyzeTestObjectFile(Program.sdaConfiguration, _logger, mode);
                        Console.WriteLine("Test object analysis completed. Press any key to continue...");
                        Console.ReadKey();
                    }),
                    new("Partial report", async () =>
                    {
                        int mode = 1;
                        await SDAExecutionService.AnalyzeTestObjectFile(Program.sdaConfiguration, _logger, mode);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();})
                }),
                new("Compare Baselines", new List<MenuItem>
                {
                    new("Generate Test objects to Compare", async () =>
                    {
                        Console.WriteLine("Comparing baselines (Baseline #2 is evaluated/compared in terms of Baseline #1)...");

                        // Ask the user for the baseline files to compare
                        Console.WriteLine("Please enter the Name of Baseline #1 (default: SPA2_INT):");
                        string baseline1 = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(baseline1))
                        {
                            baseline1 = "SPA2_INT";
                        }

                        Console.WriteLine("Please enter the Name of Baseline #2 (default: 725B_TT2_110_RC_INT):");
                        string baseline2 = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(baseline2))
                        {
                            baseline2 = "725B_TT2_110_RC_INT";
                        }

                        await Task.Run(() => BaselineService.CompareBaselines(Program.sdaConfiguration, this._availableLoggers, baseline1, baseline2));

                        compareBaselineFile1 = BaselineService.outputFileName1 ?? string.Empty;
                        compareBaselineFile2 = BaselineService.outputFileName2 ?? string.Empty;

                        Console.WriteLine("Generation of Test objects completed. Press any key to continue...");
                        Console.ReadKey();
                    }),
                    new("Analyze results", async () =>
                    {

                        if (string.IsNullOrWhiteSpace(compareBaselineFile1) || string.IsNullOrWhiteSpace(compareBaselineFile2))
                        {
                            // Ask the user for the test object files to compare
                            Console.WriteLine("Please enter the path to the first test object file:");

                            string outputDirectory = Path.GetDirectoryName("./output/") ?? string.Empty;
                            if (!Directory.Exists(outputDirectory))
                            {
                                _logger.LogError($"Output directory not found: {outputDirectory}");
                                return;
                            }

                            var files = Directory.GetFiles(outputDirectory, "*.json");
                            if (files.Length == 0)
                            {
                                _logger.LogError("No test object files found in the output directory.");
                                return;
                            }

                            Console.WriteLine("Available test object files:");
                            for (int i = 0; i < files.Length; i++)
                            {
                                Console.WriteLine($"{i + 1}: {Path.GetFileName(files[i])}");
                            }

                            Console.Write("Select a file 1 to analyze (enter the number): ");
                            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > files.Length)
                            {
                                _logger.LogError("Invalid selection.");
                                return;
                            }

                            compareBaselineFile1 = files[selectedIndex - 1];

                            if (!File.Exists(compareBaselineFile1))
                            {
                                _logger.LogError($"Test object file not found at {compareBaselineFile1}");
                                return;
                            }

                            Console.WriteLine("Available test object files:");
                            for (int i = 0; i < files.Length; i++)
                            {
                                Console.WriteLine($"{i + 1}: {Path.GetFileName(files[i])}");
                            }

                            Console.Write("Select a file 2 to analyze (enter the number): ");
                            if (!int.TryParse(Console.ReadLine(), out int selectedIndex2) || selectedIndex2 < 1 || selectedIndex2 > files.Length)
                            {
                                _logger.LogError("Invalid selection.");
                                return;
                            }

                            compareBaselineFile2 = files[selectedIndex2 - 1];

                            if (!File.Exists(compareBaselineFile1))
                            {
                                _logger.LogError($"Test object file not found at {compareBaselineFile1}");
                                return;
                            }


                            if (string.IsNullOrWhiteSpace(compareBaselineFile1) || string.IsNullOrWhiteSpace(compareBaselineFile2))
                            {

                                Console.WriteLine("Please generate the test objects first.");
                                return;
                            }
                        }

                        Console.WriteLine("Analyzing compatibility of software...");

                        // Ask the user if the want the results to be exported to an excel file as Comma Separated Values (CSV)
                        Console.WriteLine("Do you want to export the results to an excel file? (y/n)");
                        string exportToExcel = Console.ReadLine();
                        if (exportToExcel?.ToLower() == "y")
                        {
                            await Task.Run(() => BaselineService.CompatibilitySoftwareCheck(Program.sdaConfiguration, this._availableLoggers, compareBaselineFile1, compareBaselineFile2, true));
                        }
                        else
                        {
                            await Task.Run(() => BaselineService.CompatibilitySoftwareCheck(Program.sdaConfiguration, this._availableLoggers, compareBaselineFile1, compareBaselineFile2));
                        }

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    })
                }),

                new("Help", new List<MenuItem>
                {
                    new("Controls Guide", async () => await Task.Run(() => HelpService.ControlsGuide())),

                }),
                new("Exit", async () => await Task.Run(() => Environment.Exit(0)))
            });
        }
    }
}
