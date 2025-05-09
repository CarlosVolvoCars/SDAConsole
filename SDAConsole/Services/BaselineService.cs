using Microsoft.Extensions.Logging;
using SDAConsole.types;
using System.Text.Json;
using SDAConsole.exporters;

namespace InteractiveApp.Services
{
    public static class BaselineService
    {
        public static string? outputFileName1;
        public static string? outputFileName2;
        public static void CompareBaselines(SDAConfiguration config, List<ILogger> availableLoggers, string baseline1, string baseline2)
        {
            var logger = availableLoggers[0];

            Console.WriteLine($"Comparing {baseline1} with {baseline2}...");

            List<(string, string)> args = new List<(string, string)>
            {
                ("TargetBaselineNameOverride", baseline1)
            };

            SDAExecutionService.CreateTestObjectFile(availableLoggers, args).Wait();

            string pathResults1 = SDAExecutionService.GetOutputFileName();

            logger.LogInformation($"Test object file #1 created at {pathResults1}");

            var target = args.FirstOrDefault(x => x.Item1 == "TargetBaselineNameOverride");

            if (target != default)
            {
                args.Remove(target);
                args.Add(("TargetBaselineNameOverride", baseline2));
            }

            SDAExecutionService.CreateTestObjectFile(availableLoggers, args).Wait();

            string pathResults2 = SDAExecutionService.GetOutputFileName();

            logger.LogInformation($"Test object file #2 created at {pathResults2}");

            outputFileName1 = pathResults1;
            outputFileName2 = pathResults2;

            // Read the generated test object files

            // Run the comparison logic
            // Which will itereate through the test object files CoMoExpressions and compare them by ECU-SW type-and-part number
            // and then output the differences as txt file that has a table that displays ECUs per line and the differences in the CoMoExpressions
        }

        public static async Task CompatibilitySoftwareCheck(SDAConfiguration config, List<ILogger> availableLoggers, string testObject1Path, string testObject2Path, bool exportToFile = false)
        {
            var logger = availableLoggers[0];
            List<(string nodeName, bool evaluation)> sameSoftwareOnECUList1 = [];
            List<(string nodeName, bool evaluation)> sameSoftwareOnECUList2 = [];

            List<(string nodeName, string SWType, string SWPart, string CoMo, string evaluation)> evaluationDetails1 = [];
            List<(string nodeName, string SWType, string SWPart, string CoMo, string evaluation)> evaluationDetails2 = [];

            if (!File.Exists(testObject1Path) || !File.Exists(testObject2Path))
            {
                logger.LogError($"Test object file not found at {testObject1Path} or {testObject2Path}");
                return;
            }

            CsvExporter ExportFactory = new CsvExporter();

            // Read the generated test object files

            try
            {
                string testObject1json = await File.ReadAllTextAsync(testObject1Path);
                string testObject2json = await File.ReadAllTextAsync(testObject2Path);

                TestObjectConfiguration testObj1 = JsonSerializer.Deserialize<TestObjectConfiguration>(testObject1json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false
                })?? throw new InvalidOperationException("Failed to deserialize testObject1json.");

                TestObjectConfiguration testObj2 = JsonSerializer.Deserialize<TestObjectConfiguration>(testObject2json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = false
                })?? throw new InvalidOperationException("Failed to deserialize testObject2json.");

                if (testObj1 == null || testObj2 == null)
                {
                    logger.LogError("Failed to deserialize test object files.");
                    return;
                }

                // Compare CoMoExpressions between the two test objects and output differences
                Console.WriteLine($"\nComparing CoMo Expressions FROM {testObject1Path} TO {testObject2Path}\n");
                Console.WriteLine($"{"Reference Baseline Name: ",-25}{testObj1.ReferenceBaselineName}");

                var tableHeader = $"{"ECU",-25}{"SW Part Type",-20}{"SW Part Number",-20}{"CoMo Expression",-60}{$"Match in {testObj2.ReferenceBaselineName}",-60}";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(tableHeader);
                Console.WriteLine(new string('-', tableHeader.Length));
                Console.ResetColor();

                var ecuDifferences = new List<string>();

                foreach (var ecu1 in testObj1.CoMoExpressions)
                {
                    var ecu2 = testObj2.CoMoExpressions.FirstOrDefault(e => e.Comment == ecu1.Comment);

                    logger.LogInformation($"Comparing {ecu1.Comment} with {ecu2?.Comment}");

                    string como1 = $"{ecu1.Context}-{ecu1.ecuSolutionParameter}-{string.Join("-", ecu1.ConfigurationParameterValues)}";
                    string como2 = ecu2 != null ? $"{ecu2.Context}-{ecu2.ecuSolutionParameter}-{string.Join("-", ecu2.ConfigurationParameterValues)}" : "N/A";

                    string ecuName = ecu1.Comment.Split('-')[0].Trim();
                    string ecuPartType = ecu1.Comment.Split('-')[1].Trim();
                    string ecuPartNumber = ecu1.Comment.Split('-')[2].Trim();

                    if (ecu2 == null)
                    {
                        ecuDifferences.Add($"ECU {ecu1.Comment} is missing in the second test object.");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como1,-60}{"Missing",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList1 = buildSameSoftwareOnECUList(sameSoftwareOnECUList1, ecuName, false);
                        evaluationDetails1.Add((ecuName, ecuPartType, ecuPartNumber, como1, "Missing"));
                        continue;
                    }

                    if (como1 != como2)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como1,-60}{"Match with different CoMo",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList1 = buildSameSoftwareOnECUList(sameSoftwareOnECUList1, ecuName, true);
                        evaluationDetails1.Add((ecuName, ecuPartType, ecuPartNumber, como1, "Match with different CoMo"));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como1,-60}{"Match",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList1 = buildSameSoftwareOnECUList(sameSoftwareOnECUList1, ecuName, true);
                        evaluationDetails1.Add((ecuName, ecuPartType, ecuPartNumber, como1, "Match"));
                    }
                }
                Console.WriteLine(new string('-', tableHeader.Length));

                Console.WriteLine($"\nComparing CoMo Expressions FROM {testObject2Path} TO {testObject1Path}\n");
                Console.WriteLine($"{"Reference Baseline Name: ",-25}{testObj2.ReferenceBaselineName}");

                tableHeader = $"{"ECU",-25}{"SW Part Type",-20}{"SW Part Number",-20}{"CoMo Expression",-60}{$"Match in {testObj1.ReferenceBaselineName}",-60}";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(tableHeader);
                Console.WriteLine(new string('-', tableHeader.Length));
                Console.ResetColor();

                foreach (var ecu2 in testObj2.CoMoExpressions)
                {
                    var ecu1 = testObj1.CoMoExpressions.FirstOrDefault(e => e.Comment == ecu2.Comment);

                    logger.LogInformation($"Comparing {ecu2.Comment} with {ecu1?.Comment}");

                    string como2 = $"{ecu2.Context}-{ecu2.ecuSolutionParameter}-{string.Join("-", ecu2.ConfigurationParameterValues)}";
                    string como1 = ecu1 != null ? $"{ecu1.Context}-{ecu1.ecuSolutionParameter}-{string.Join("-", ecu1.ConfigurationParameterValues)}" : "N/A";

                    string ecuName = ecu2.Comment.Split('-')[0].Trim();
                    string ecuPartType = ecu2.Comment.Split('-')[1].Trim();
                    string ecuPartNumber = ecu2.Comment.Split('-')[2].Trim();

                    if (ecu1 == null)
                    {
                        ecuDifferences.Add($"ECU {ecu2.Comment} is missing in the first test object.");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como2,-60}{"Missing",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList2 = buildSameSoftwareOnECUList(sameSoftwareOnECUList2, ecuName, false);
                        evaluationDetails2.Add((ecuName, ecuPartType, ecuPartNumber, como2, "Missing"));
                        continue;
                    }

                    if (como1 != como2)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como2,-60}{"Match with different CoMo",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList2 = buildSameSoftwareOnECUList(sameSoftwareOnECUList2, ecuName, true);
                        evaluationDetails2.Add((ecuName, ecuPartType, ecuPartNumber, como2, "Match with different CoMo"));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{ecuName,-25}{ecuPartType,-20}{ecuPartNumber,-20}{como2,-60}{"Match",-60}");
                        Console.ResetColor();
                        sameSoftwareOnECUList2 = buildSameSoftwareOnECUList(sameSoftwareOnECUList2, ecuName, true);
                        evaluationDetails2.Add((ecuName, ecuPartType, ecuPartNumber, como2, "Match"));
                    }
                }

                Console.WriteLine(new string('-', tableHeader.Length));

                if (ecuDifferences.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nDifferences found:\n");
                    foreach (var diff in ecuDifferences)
                    {
                        Console.WriteLine(diff);
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nNo differences found in SW PNs fetched when comparing both Baselines.");
                    Console.ResetColor();
                }

                Console.WriteLine("\n\nSummary of ECU evaluations 1:\n");
                Console.WriteLine($"{"ECU",-25}{"Evaluation",-20}");
                Console.WriteLine(new string('-', 45));
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var ecu in sameSoftwareOnECUList1)
                {
                    Console.WriteLine($"{ecu.nodeName,-25}{ecu.evaluation,-20}");
                }
                Console.WriteLine(new string('-', 45));
                Console.ResetColor();


                Console.WriteLine("\n\nSummary of ECU evaluations 2:\n");
                Console.WriteLine($"{"ECU",-25}{"Evaluation",-20}");
                Console.WriteLine(new string('-', 45));
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var ecu in sameSoftwareOnECUList2)
                {
                    Console.WriteLine($"{ecu.nodeName,-25}{ecu.evaluation,-20}");
                }
                Console.WriteLine(new string('-', 45));
                Console.ResetColor();

                if (exportToFile)
                {
                    Console.WriteLine("Exporting results to Excel...");
                    ExportFactory.ExportResultsToExcel(
                        Program.sdaConfiguration.outputFolderPath + Program.sdaConfiguration.exportedExcelFilePath,
                        sameSoftwareOnECUList1,
                        sameSoftwareOnECUList2,
                        ecuDifferences,
                        evaluationDetails1,
                        evaluationDetails2,
                        testObj1.ReferenceBaselineName,
                        testObj2.ReferenceBaselineName
                    );
                    Console.WriteLine($"Results exported to {Program.sdaConfiguration.outputFolderPath + Program.sdaConfiguration.exportedExcelFilePath}");
                }


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to analyze test object files.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error analyzing test object files. Check logs for more info.");
                Console.ResetColor();
            }



        }

        private static List<(string nodeName, bool evaluation)> buildSameSoftwareOnECUList(List<(string nodeName, bool evaluation)> sameSoftwareOnECUListInput, string ecuName, bool evaluationResult)
        {
            bool found = false;

            for (int i = 0; i < sameSoftwareOnECUListInput.Count; i++)
            {
                var ecu = sameSoftwareOnECUListInput[i];
                if (ecu.nodeName == ecuName)
                {
                    // Update the evaluation result if the ECU already exists
                    if (ecu.evaluation != false)
                    {
                        // Update the evaluation result if the ECU already exists
                        sameSoftwareOnECUListInput[i] = (ecu.nodeName, evaluationResult);
                        found = true;
                        break;
                    }

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // Add the ECU name to the list with its evaluation result if not found
                sameSoftwareOnECUListInput.Add((ecuName, evaluationResult));
            }

            return sameSoftwareOnECUListInput;
        }

    }



}
