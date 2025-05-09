using System.Text.Json;
using DAL.VehicleFinder;
using DAL.Connection;
using DAL.Shared.Models;
using DAL.CallSiiG;
using DAL.SIIGOrderCreator;
using DAL.SIIGOrderCreator.Models;
using DAL.Instruction;
using SDAConsole.types;
using SDAConsole.ConfigHub;
using SDAConsole.types.ConfigHub;
using SDAConsole.interfaces.API;
using SDAConsole.types.ConfigurationModeller;
using SDAConsole.services;
using SDAConsole.loaders;
using SDAConsole.FileSystemHandlers;

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;

using InteractiveApp.InteractiveMenu;
using InteractiveApp.Config;

using ShellProgressBar;
using Pastel;
using System.ComponentModel.DataAnnotations;


public class SDAException : Exception
{
    public SDAException(string message) : base(message) { }
    public SDAException(string message, Exception innerException) : base(message, innerException) { }
}

class Program //SDA – Software Download Assistant
{
    private static string timestampPrefixToNameLogFiles = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    private static string outputFileName;
    public static SDAConfiguration? sdaConfiguration;
    public static ConfigHubConfiguration? configHubConfiguration;


    static async Task Main(string[] args)
    {

        // Serilog logger for [SDA]
        var serilogLoggerSDA = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            // .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .WriteTo.File($"./logs/{timestampPrefixToNameLogFiles}_SDA.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
            .CreateLogger();

        // Serilog logger for [SDA][VehicleFinder]
        var serilogLoggerVehicleFinder = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File($"./logs/{timestampPrefixToNameLogFiles}_VehicleFinder.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
            .CreateLogger();

        // TODO: Create a Serilog logger factory for each logger, for example one for ConfigHub...

        var factorySDA = new LoggerFactory().AddSerilog(serilogLoggerSDA);
        var loggerSDA = factorySDA.CreateLogger("[SDA]");

        var factoryVehicleFinder = new LoggerFactory().AddSerilog(serilogLoggerVehicleFinder);
        var loggerVehicleFinder = factoryVehicleFinder.CreateLogger("[SDA][VehicleFinder]");

        // Add loggers to a list
        List<Microsoft.Extensions.Logging.ILogger> availableLoggers = new List<Microsoft.Extensions.Logging.ILogger> { loggerSDA, loggerVehicleFinder };

        string mainConfigPath = "./configs/configuration.json";
        // string mainConfigPath = "../../../configs/configurationTest.json";

        sdaConfiguration = SDAConfigurationLoader.LoadFromFile(mainConfigPath);
        var version = File.ReadAllText("./configs/version.txt");
        // var version = File.ReadAllText("../../../configs/version.txt");
        loggerSDA.LogInformation($"Version: {version}");

        // Parse arguments

        var options = SDAArgsParser.Parse(args);

        if (options.ShowHelp)
        {
            // Print help and exit, use Pastel for colors and formatting
            Console.WriteLine("Usage: SDAConsole [options]".Pastel(ConsoleColor.Cyan));
            Console.WriteLine("Options:".Pastel(ConsoleColor.Cyan));
            Console.WriteLine("--config <path>                      Path to the DAL configuration file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--testObjectTemplatePath <path>      Path to the test object template configuration file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--baseCarConfigPath <path>           Path to the base car configuration file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--baseInstructionPath <path>         Path to the base instruction file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--baseInstructionReportPath <path>   Path to the base instruction report file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--testObjectOutputPath <path>        Path to the test object output file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--ecuListFilePath <path>             Path to the ECU list file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--vehicleTypeCodesFilePath <path>    Path to the vehicle type codes file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--vehicleFirstReadoutFilePath <path> Path to the vehicle first readout file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--configHubSettingsPath <path>       Path to the ConfigHub settings file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--excludedEcusFilePath <path>        Path to the excluded ECUs file.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--interactiveMode                    Run in interactive mode. (Advanced users only)".Pastel(ConsoleColor.Green));
            Console.WriteLine("--version                            Show version information.".Pastel(ConsoleColor.Green));
            Console.WriteLine("--help                               Show this help message.".Pastel(ConsoleColor.Green));
            return;

        }
        if (options.ShowVersion)
        {
            Console.WriteLine($"Version {version}");
            return;
        }

        if (options.InteractiveMode)
        {
            var menuManager = new MenuManager(availableLoggers);
            await menuManager.Run();
            return;
        }

        await RunSDAConsole(availableLoggers, false);
    }

    public static async Task RunSDAConsole(List<Microsoft.Extensions.Logging.ILogger> availableLoggers, bool isInteractive = false, List<(string, string)> args = null)
    {
        // Deconstruct availableLoggers to get the logger for the current context, remember that the list is structure like new List<ILogger> { logger, vehicleFinderLogger };
        Microsoft.Extensions.Logging.ILogger logger = availableLoggers[0];
        Microsoft.Extensions.Logging.ILogger vehicleFinderLogger = availableLoggers[1];

        // SDA Config deconstruction
        if (sdaConfiguration == null)
        {
            throw new ArgumentNullException(nameof(sdaConfiguration), "SDA configuration is null.");
        }

        string configPath = sdaConfiguration.dalAppSettingsPath;
        string templatePath = sdaConfiguration.testObjectTemplateConfigPath;
        string baseCarConfigPath = sdaConfiguration.baseCarConfigPath;
        string instructionPath = sdaConfiguration.baseInstructionPath;
        string instructionReportPath = sdaConfiguration.baseInstructionReportPath;
        string testObjectOutputPath = sdaConfiguration.outputFolderPath + sdaConfiguration.testObjectOutputFilePath;
        string ecuListFilePath = sdaConfiguration.ecuListFilePath;
        string vehicleTypeCodesFilePath = sdaConfiguration.vehicleTypeCodesFilePath;
        string vehicleFirstReadoutFilePath = sdaConfiguration.vehicleFirstReadoutFilePath;
        string confighubSettingsPath = sdaConfiguration.confighubSettingsPath;
        string dataStorePath = "./dataStore";
        // string dataStorePath = "../../../dataStore";
        string logsPath = "./logs";
        // string logsPath = "../../../logs";



        Console.WriteLine($"{Environment.NewLine}SDA Console - Software Download Assistant Console".Pastel(ConsoleColor.Cyan));
        Console.WriteLine($"Version: {File.ReadAllText("./configs/version.txt")}".Pastel(ConsoleColor.Cyan));
        Console.WriteLine($"Date: {DateTime.Now} \n".Pastel(ConsoleColor.Cyan));

        // Create a progress bar for the main process

        int maxTicks = 19;
        using var mainProgress = new ProgressHelper("SDA Run", maxTicks);

        DirectoriesHandler directoriesHandler = new DirectoriesHandler(logger, mainProgress.Bar);

        // Cleans the dataStore folder from files, keeps folders
        mainProgress.Tick("Step 0: Clean dataStore folder");
        directoriesHandler.EnsureDirectoryAndClearFiles(dataStorePath);

        mainProgress.Tick("Step 1: Clean logs folder");
        directoriesHandler.EnsureDirectoryAndClearFiles(logsPath, timestampPrefixToNameLogFiles);

        // Load ConfigHub settings
        mainProgress.Tick("Step 2: Load ConfigHub settings");
        configHubConfiguration = ConfigHubConfigurationLoader.LoadFromFile(confighubSettingsPath);

        // Instantiate ConfigHub/API helpers
        IApiServerSelector selector = new ConfigHubServerSelector(configHubConfiguration);
        IApiClient apiClient = new ApiClientController(new HttpClient());

        // Create ConfigHub API client
        var configHubClient = new ConfigHubClient(configHubConfiguration, apiClient, logger);

        // First time login to ConfigHub to get the token
        await configHubClient.SignInAsync(configHubConfiguration.UserName, configHubConfiguration.Password, ServerInstanceOption.PROD, token: null);

        // Get BaseLineId
        // If args have a value 'TargetBaselineNameOverride' use it to get the BaseLineId, else use the one from the config file
        if (args != null && args.Count > 0)
        {
            foreach (var arg in args)
            {
                if (arg.Item1 == "TargetBaselineNameOverride")
                {
                    configHubConfiguration.TargetBaselineName = arg.Item2;
                    await configHubClient.GetBaseLineIdByName(arg.Item2);
                }
            }
        }else
        {
            await configHubClient.GetBaseLineIdByName(configHubConfiguration.TargetBaselineName);
        }


        // Extract ECU SubBaseline IDs
        var subBaselines = await configHubClient.GetSubBaselinesFromMetadata();

        try
        {
            // Load configurations
            mainProgress.Tick("Step 3: Load DAL config");
            DalConfiguration config = DalConfigurationLoader.LoadFromFile(configPath, mainProgress.Bar);

            mainProgress.Tick("Step 4: Load test object template config");
            TestObjectConfiguration testObjectDataSchema = TestObjectConfigurationLoader.LoadFromFile(templatePath, mainProgress.Bar);

            testObjectDataSchema.ReferenceBaselineURL = configHubConfiguration.TargetBaselineID;

            testObjectDataSchema.ReferenceBaselineName = configHubConfiguration.TargetBaselineName;

            mainProgress.Tick("Step 5: Load ECU list config");
            List<ECUList> ecuListData = EcuListLoader.LoadFromFile(ecuListFilePath, mainProgress.Bar);

            mainProgress.Tick("Step 6: Load vehicle type codes config");
            List<VehicleTypeCodes> vehicleTypeCodes = VehicleTypeCodesLoader.LoadFromFile(vehicleTypeCodesFilePath, mainProgress.Bar);

            var vehicleFinder = new VehicleFinderFactory(vehicleFinderLogger).Create();
            mainProgress.Tick("Step 7: Starting vehicle search...");
            logger.LogInformation("Starting vehicle search...");

            var announcedVehicle = await vehicleFinder.FindFirstVehicleAndStopSearch(10000);
            if (announcedVehicle == null)
            {
                logger.LogWarning("No vehicle found. Ensure the vehicle is powered on and connected.");
                return;
            }

            logger.LogInformation($"Vehicle found at {announcedVehicle.Ip}");

            mainProgress.Tick("Step 8: Creation of Vehicle Connection complete");
            var vehicleConnection = new VehicleConnectionFactory().Create(announcedVehicle.Ip, vehicleFinderLogger);
            logger.LogInformation("Connected to vehicle.");

            // Initial readout
            logger.LogInformation("First Readout (RAW UDS requests).");
            mainProgress.Tick("Step 9: Perform First Readout (RAW UDS requests).");
            var readoutService = new ReadoutService(vehicleConnection, config, logger);
            var readoutServiceResult = await readoutService.GetReadoutReportFromSingleDiagRequest("22F186", 0x1FFF);

            if (readoutServiceResult.Report == null && readoutServiceResult.Response == null)
            {
                logger.LogError("Failed to retrieve vehicle readout.");
                return;
            }else
            {
                logger.LogInformation("Readout completed successfully.");
            }

            var readoutReport = readoutServiceResult.Report;
            var readoutResponse = readoutServiceResult.Response;

            var firstReadoutResponse = JsonSerializer.Serialize(readoutResponse, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(vehicleFirstReadoutFilePath, firstReadoutResponse);

            if (readoutReport == null)
            {
                logger.LogError("Failed to retrieve vehicle readout.");
                return;
            }

            // Generates the Vehicle Object
            logger.LogInformation("Generating VehicleObject...");
            var vehicleObjectService = new VehicleObjectService(config, vehicleFinderLogger);
            var vehicleObject = await vehicleObjectService.GenerateVehicleObjectAsync(readoutReport);

            if (vehicleObject == null)
            {
                logger.LogError("Failed to generate VehicleObject.");
                return;
            }

            mainProgress.Tick("Step 10: Generating Vehicle Object complete");

            // Create SiiG order to use as input for siigClient.GetInstruction
            logger.LogInformation("Creating SiiG Order Generator...");
            var vbfFilePaths = new List<string> { baseCarConfigPath };
            var siigOrderGenerator = new SiiGOrderGeneratorFactory(config, vehicleFinderLogger).Create();

            var siigOrder = siigOrderGenerator.CreateSiiGOrder(new OrderOptions() {

                VbfFilesPath = vbfFilePaths

            });

            mainProgress.Tick("Step 11: Creation of SiiG Order complete");

            // Download instruction from SIIG
            logger.LogInformation("Downloading instruction from SIIG...");
            var siigClient = new SiiGClientFactory(config, vehicleFinderLogger).Create();
            var instruction = await siigClient.GetInstruction(siigOrder, vehicleObject);

            mainProgress.Tick("Step 12: Download of instruction from SIIG complete");

            // Save instruction to file from instruction.Text
            var instructionFilePath = instructionPath;
            File.WriteAllText(instructionFilePath, instruction.Text);

            // Instruction created previously
            // Execute instruction against vehicle
            var instructionService = new InstructionService(logger);
            var resultFromInstructionReadout = instructionService.ExecuteInstruction(vehicleConnection, instructionFilePath);

            mainProgress.Tick("Step 13: Execution instruction against vehicle complete");
            // logger.LogInformation("Execution resultFromInstructionReadout: {0}", JsonSerializer.Serialize(resultFromInstructionReadout, new JsonSerializerOptions { WriteIndented = true }));

            // Parse results into the VehicleObject
            logger.LogInformation("Parsing results into VehicleObject...");
            mainProgress.Tick("Step 14: Parsing results into VehicleObject");

            var readoutResultsFilePath = instructionReportPath;
            File.WriteAllText(readoutResultsFilePath, JsonSerializer.Serialize(resultFromInstructionReadout, new JsonSerializerOptions { WriteIndented = true }));

            ReadoutReport installationReportDataSchema = InstallationReportLoader.LoadFromFile(readoutResultsFilePath);
            mainProgress.Tick($"Step 15: Readout Report from IM {config.UseCase.Siig}");

            // Extract Data from InstallationReport
            logger.LogInformation("Extracting data from InstallationReport...");
            mainProgress.Tick($"Step 16: Parse InstallationReports object from Report");
            InstallationReportParser parser = new InstallationReportParser(installationReportDataSchema.InstallationReports[0], config, logger);

            var vehicleIdentificationNumber = await parser.GetVehicleIdentificationNumber(mainProgress.Bar);

            var TypeCode = await parser.GetVehicleType(mainProgress.Bar);

            var fyon = await parser.GetFactoryOrderNumber(mainProgress.Bar);

            var chassis = await parser.GetChassisNumber(mainProgress.Bar);

            var factoryCode = await parser.GetFactoryCode(mainProgress.Bar);

            var structureWeek = await parser.GetStructureWeek(mainProgress.Bar);

            var coMoExpresions = await parser.GetConfigurationModellerExpresions(sdaConfiguration.steeringCoMoExpressions, mainProgress.Bar);
            var coMoExpresionsFileContent = JsonSerializer.Serialize(coMoExpresions, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"./dataStore/coMoExpresions.json", coMoExpresionsFileContent);
            // File.WriteAllText($"../../../dataStore/coMoExpresions.json", coMoExpresionsFileContent);

            var ecuList = await parser.GetEcuList(ecuListData);

            List<ApplDiagnosticPartNumbers> applDiagnosticPartNumberList = await parser.GetApplDiagnosticPartNumberList();

            var ApplDiagnosticPartNumbersBEFORE = JsonSerializer.Serialize(applDiagnosticPartNumberList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"./dataStore/ApplDiagnosticPartNumbersBEFORE.json", ApplDiagnosticPartNumbersBEFORE);
            //  File.WriteAllText($"../../../dataStore/ApplDiagnosticPartNumbersBEFORE.json", ApplDiagnosticPartNumbersBEFORE);

            // Assert for ApplDiagnosticPartNumberList ECUs vs Main ECUList
            readoutService.AssertApplDiagnosticPartNumberListCompletenessAgainstECUList(ecuList, applDiagnosticPartNumberList, parser).Wait();

            var ApplDiagnosticPartNumbersAFTER = JsonSerializer.Serialize(applDiagnosticPartNumberList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"./dataStore/ApplDiagnosticPartNumbersAFTER.json", ApplDiagnosticPartNumbersAFTER);
            // File.WriteAllText($"../../../dataStore/ApplDiagnosticPartNumbersAFTER.json", ApplDiagnosticPartNumbersAFTER);

            List<HardwarePartNumbers> hardwarePartNumberList = await parser.GetHardwarePartNumberList();

            var HardwarePartNumbersBEFORE = JsonSerializer.Serialize(hardwarePartNumberList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"./dataStore/HardwarePartNumbersBEFORE.json", HardwarePartNumbersBEFORE);
            // File.WriteAllText($"../../../dataStore/HardwarePartNumbersBEFORE.json", HardwarePartNumbersBEFORE);

            // Assert for HardwarePartNumberList ECUs vs Main ECUList
            readoutService.AssertHardwarePartNumberListCompletenessAgainstECUList(ecuList, hardwarePartNumberList, parser).Wait();

            var HardwarePartNumbersAFTER = JsonSerializer.Serialize(hardwarePartNumberList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText($"./dataStore/HardwarePartNumbersAFTER.json", HardwarePartNumbersAFTER);
            // File.WriteAllText($"../../../dataStore/HardwarePartNumbersAFTER.json", HardwarePartNumbersAFTER);

            // Add VIN to VehicleObject
            vehicleObject.Vehicle.Vin = vehicleIdentificationNumber;
            logger.LogInformation("VehicleObject VIN updated: {Vin}", vehicleObject.Vehicle.Vin.ToString());

            // Add VehicleTypes to VehicleObject
            vehicleObject.Vehicle.TypeCode = TypeCode;
            logger.LogInformation("VehicleObject VehicleType updated: {0}", vehicleObject.Vehicle.TypeCode);

            // Add FYON to VehicleObject
            vehicleObject.Vehicle.Fyon = fyon;
            logger.LogInformation("VehicleObject FYON updated: {0}", vehicleObject.Vehicle.Fyon);

            // Add Chassis to VehicleObject
            vehicleObject.Vehicle.ChassisNumber = chassis;
            logger.LogInformation("VehicleObject Chassis updated: {0}", vehicleObject.Vehicle.ChassisNumber);

            // Add FactoryCode to VehicleObject
            vehicleObject.Vehicle.FactoryCode = factoryCode;
            logger.LogInformation("VehicleObject FactoryCode updated: {0}", vehicleObject.Vehicle.FactoryCode);

            // Add StructureWeek to VehicleObject
            vehicleObject.Vehicle.StructureWeek = structureWeek;
            logger.LogInformation("VehicleObject StructureWeek updated: {0}", vehicleObject.Vehicle.StructureWeek);

            // Add CoMoExpresions to VehicleObject
            testObjectDataSchema.AddCoMoExpressionsFromList("", "", "", coMoExpresions);
            logger.LogInformation("VehicleObject Number of CoMoExpressions updated: {0}", testObjectDataSchema.CoMoExpressions.Count);

            // Add ECUList to VehicleObject
            testObjectDataSchema.AddEcusFromList(ecuList, applDiagnosticPartNumberList, hardwarePartNumberList, sdaConfiguration.excludedECUListFilePath);

            // if there are items in sdaConfiguration.forcedHardwarePartNumbers check for matches of EcuName on each of the items of the list testObjectDataSchema.EcuList property Ecu.
            // if a match exist replace EcuHardwarePartNumber and EcuHardwarePartVersion from the item of the list sdaConfiguration.forcedHardwarePartNumbers with the values from the item of the list testObjectDataSchema.EcuList property HardwarePartNumber and Version respectively.
            if (sdaConfiguration.forcedHardwarePartNumbers != null && sdaConfiguration.forcedHardwarePartNumbers.Count > 0)
            {
                foreach (var forcedHardwarePartNumber in sdaConfiguration.forcedHardwarePartNumbers)
                {

                        foreach (var ecu in testObjectDataSchema.EcuList)
                        {
                            if (ecu.Ecu == forcedHardwarePartNumber.EcuName)
                            {
                                ecu.HardwarePartNumber = forcedHardwarePartNumber.EcuHardwarePartNumber;
                                ecu.Version = forcedHardwarePartNumber.EcuHardwarePartVersion;
                                logger.LogInformation($"Forced Hardware Part Number updated for ECU {ecu.Ecu}: {ecu.HardwarePartNumber} - {ecu.Version}");
                            }
                        }

                }
            }

            if (configHubConfiguration != null)
            {
                var compatibleECUListFromTargetBaseline = await parser.GetCompatibleSoftwareFilteredByECUListFromTargetBaseline(testObjectDataSchema.EcuList, subBaselines, configHubClient);
                var compatibleECUListFromTargetBaselineFileContent = JsonSerializer.Serialize(compatibleECUListFromTargetBaseline, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"./dataStore/GetCompatibleECUListFromTargetBaseline.json", compatibleECUListFromTargetBaselineFileContent);
                // File.WriteAllText($"../../../dataStore/GetCompatibleECUListFromTargetBaseline.json", compatibleECUListFromTargetBaselineFileContent);

                var ecuConfigurationParameterValues = await parser.GetCompatibleHardwareEcuConfigurationParametersFilterByECUListFromTargetBaselines(testObjectDataSchema.EcuList, subBaselines, configHubClient);
                // Remove Duplicates from ecuConfigurationParameterValues
                var uniqueEcuConfigurationParameterValues = ecuConfigurationParameterValues.Distinct().ToList();

                var ecuConfigurationParameterValuesFileContent = JsonSerializer.Serialize(uniqueEcuConfigurationParameterValues, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"./dataStore/ecuConfigurationParameterValuesFileContent.json", ecuConfigurationParameterValuesFileContent);
                // File.WriteAllText($"../../../dataStore/ecuConfigurationParameterValuesFileContent.json", ecuConfigurationParameterValuesFileContent);



                if (uniqueEcuConfigurationParameterValues.Count > 0)
                    {
                        List<string> updatedCoMoExpresions = new List<string>(coMoExpresions);

                        foreach (var steeringCoMoExpression in uniqueEcuConfigurationParameterValues)
                        {
                            var steeringCoMoExpressionParts = steeringCoMoExpression.Split('-');
                            var steeringCoMoExpressionFirstPart = steeringCoMoExpressionParts[0];
                            var steeringCoMoExpressionAdded = false;

                            for (int i = 0; i < coMoExpresions.Count; i++)
                            {
                                var coMoExpressionParts = coMoExpresions[i].Split('-');
                                var coMoExpressionFirstPart = coMoExpressionParts[0];

                                if (steeringCoMoExpressionFirstPart == coMoExpressionFirstPart)
                                {
                                    // Replace the COMO value with the one from the ecuConfigurationParameterValues
                                    updatedCoMoExpresions[i] = $"{steeringCoMoExpression}";
                                    steeringCoMoExpressionAdded = true;
                                    break;
                                }
                            }

                            if (!steeringCoMoExpressionAdded)
                            {
                                updatedCoMoExpresions.Add(steeringCoMoExpression);
                            }
                        }

                        coMoExpresions = updatedCoMoExpresions;
                    }

                var outputCOMOsPlusHWCOMOsFileContent = JsonSerializer.Serialize(coMoExpresions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("./dataStore/outputCOMOsPlusHWCOMOsFileContent.json", outputCOMOsPlusHWCOMOsFileContent);
                // File.WriteAllText($"../../../dataStore/outputCOMOsPlusHWCOMOsFileContent.json", outputCOMOsPlusHWCOMOsFileContent);

                Result filteredResult = ConfigurationFilter.FilterAndScoreConfigurations(compatibleECUListFromTargetBaseline, coMoExpresions);
                var filteredResultFileContent = JsonSerializer.Serialize(filteredResult, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"./dataStore/FilteredResult.json", filteredResultFileContent);
                // File.WriteAllText($"../../../dataStore/FilteredResult.json", filteredResultFileContent);

                testObjectDataSchema.ClearCoMoExpressions();
                testObjectDataSchema.AddAllVariantExpressionsToTestObject(filteredResult);

                SimplifyCoMoExpressionsByNode(testObjectDataSchema);
                var simplifiedCoMoExpressionsFileContent = JsonSerializer.Serialize(testObjectDataSchema.CoMoExpressions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"./dataStore/simplifiedCoMoExpressions.json", simplifiedCoMoExpressionsFileContent);
                // File.WriteAllText($"../../../dataStore/simplifiedCoMoExpressions.json", simplifiedCoMoExpressionsFileContent);

                // Add Forced CoMoExpressions if any
                if (sdaConfiguration.forcedCoMoExpressions != null && sdaConfiguration.forcedCoMoExpressions.Count > 0)
                {
                    foreach (var forcedCoMoExpression in sdaConfiguration.forcedCoMoExpressions)
                    {
                        testObjectDataSchema.AddCoMoExpression(forcedCoMoExpression.Comment, forcedCoMoExpression.Context, forcedCoMoExpression.ecuSolutionParameter, forcedCoMoExpression.ConfigurationParameterValues);
                    }
                }



            }

            // Generate JSON Test Object File
            mainProgress.Tick("Step 17: Generating JSON Test Object File");
            var jsonGenerator = new JsonConfigGenerator(logger, testObjectDataSchema, testObjectOutputPath);
            jsonGenerator.GenerateJson(vehicleObject, vehicleTypeCodes);

            outputFileName = jsonGenerator.GetFilePath();

            vehicleConnection.Close();

            if (isInteractive)
            {
                mainProgress.Tick("Step 18: Test object file generated successfully.");
                return;
            }
            else
            {
                mainProgress.Tick("Step 18: Test object file generated successfully.");
                logger.LogInformation("test_object_config.json generated successfully.");
                Environment.Exit(0);
            }

        }
        catch (SDAException ex)
        {
            logger.LogError($"[Handled] {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError($"[Unhandled] Unexpected error: {ex}");
        }
    }

    public static string GetOutputFileName()
    {
        return outputFileName;
    }

    public static void SimplifyCoMoExpressionsByNode(TestObjectConfiguration config)
    {
        if (config == null || config.CoMoExpressions == null || config.CoMoExpressions.Count == 0)
            return;

        var aggregatedExpressions = config.CoMoExpressions
            .Where(expr => !string.IsNullOrWhiteSpace(expr.Comment)
                        && !string.IsNullOrWhiteSpace(expr.Context)
                        && !string.IsNullOrWhiteSpace(expr.ecuSolutionParameter))
            .GroupBy(expr =>
            {
                var nodeName = expr.Comment.Split('-').FirstOrDefault()?.Trim();
                return $"{nodeName}|{expr.Context.Trim()}|{expr.ecuSolutionParameter.Trim()}";
            })
            .Select(group =>
            {
                var mergedComment = string.Join(", ", group.Select(e => e.Comment).Distinct());
                var mergedValues = group
                    .SelectMany(e => e.ConfigurationParameterValues ?? new List<string>())
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct()
                    .ToList();

                var first = group.First();

                return new TestObjectConfiguration.CoMoExpression
                {
                    Comment = mergedComment,
                    Context = first.Context,
                    ecuSolutionParameter = first.ecuSolutionParameter,
                    ConfigurationParameterValues = mergedValues
                };
            })
            .ToList();

        config.CoMoExpressions = aggregatedExpressions;
    }



}



public class JsonConfigGenerator
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    private readonly TestObjectConfiguration _testObjectDataSchema;

    private string _filePath;

    public JsonConfigGenerator(Microsoft.Extensions.Logging.ILogger logger, TestObjectConfiguration testObjectDataSchema, string filePath)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _testObjectDataSchema = testObjectDataSchema ?? throw new ArgumentNullException(nameof(testObjectDataSchema));
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public void GenerateJson(VehicleObject vehicle, List<VehicleTypeCodes> vehicleTypeCodes)
    {
        var localvehicle = vehicle.Vehicle;
        var typeDesignation = vehicleTypeCodes.Find(vehicleTypeCodesItem => vehicleTypeCodesItem.Type.Contains(localvehicle.TypeCode))?.TypeDesignation ?? "DefaultTypeDesignation";

        var outputJSONObject = new
        {
            ReferenceBaselineURL = $"https://confighub.volvocars.net/baselines/{_testObjectDataSchema.ReferenceBaselineURL}/details", // Concat _testObjectDataSchema.ReferenceBaselineURL to https://confighub.volvocars.net/baselines/{_testObjectDataSchema.ReferenceBaselineURL}/details
            ReferenceBaselineName = _testObjectDataSchema.ReferenceBaselineName,
            TypeDesignation = Int32.Parse(typeDesignation),
            Vin = localvehicle.Vin,
            Fyon = localvehicle.Fyon,
            ProductType = localvehicle.ProductType,
            TypeCode = localvehicle.TypeCode,
            EcuList = _testObjectDataSchema.EcuList.Select(ecu => new
                {
                    Ecu = ecu.Ecu,
                    Address = ecu.EcuAddress,
                    AddressDecimal = ecu.EcuAddressDecimal,
                    ApplDiagnosticPartNumber = ecu.ApplDiagnosticPartNumber ?? "XXXXXXXX AA", // Readout from F120 or default value
                    ApplDiagnosticPartNumberDefault = "XXXXXXXX AA",
                    DefaultPinCode = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
                    // SecurityCodes = new TestObjectConfiguration.EcuObject.SecurityCode[]
                    // {
                    //     new TestObjectConfiguration.EcuObject.SecurityCode
                    //     {
                    //         securityArea = "000",
                    //         code = ""
                    //     }
                    // },
                    // SoftwarePartNumbers = ecu.SoftwarePartNumbers,
                    // SlaveNodes = ecu.SlaveNodes,
                    HardwarePartNumber = ecu.HardwarePartNumber,
                    HardwareSerialNumber = ecu.HardwareSerialNumber,
                    Version = ecu.Version,
                    // EcuStatus = ecu.EcuStatus
                }),
            ExecEnvList = new TestObjectConfiguration.ExecEnvObject[]
                {
                    new TestObjectConfiguration.ExecEnvObject
                    {
                        ExecEnv = "HPAA"
                    },
                    new TestObjectConfiguration.ExecEnvObject
                    {
                        ExecEnv = "HPBA"
                    }
                },
            Vdns = new string[] { "" },
            AllVariantExpression = true,
            IsCompleteCar = true,
            IsCompleteCPVMatch = true,
            CoMoExpressions = _testObjectDataSchema.CoMoExpressions.Select(coMo => new // TODO: Create a function to build COMOs
                {
                    coMo.Comment,
                    coMo.Context,
                    coMo.ecuSolutionParameter,
                    coMo.ConfigurationParameterValues
                })
        };

        var outputJSONObjectFileContent = JsonSerializer.Serialize(outputJSONObject, new JsonSerializerOptions { WriteIndented = true });

        _logger.LogInformation($"Input Path: {_filePath}");
        var fileExtension = Path.GetExtension(_filePath);
        _logger.LogInformation($"File Extension: {fileExtension}");
        var fileDirectory = Path.GetDirectoryName(_filePath);
        _logger.LogInformation($"File Directory: {fileDirectory}");
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
        var newFileName = $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
        _logger.LogInformation($"New File Name: {newFileName}");
        _filePath = Path.Combine(fileDirectory, newFileName);
        _logger.LogInformation($"New File Path: {_filePath}");

        // Ensure the directory exists
        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        // Create the file before writing
        try
        {
            using (var fileStream = File.Create(_filePath))
            {
                // Close the file stream immediately after creation
                _logger.LogInformation($"Generated JSON file: {_filePath}");

            }
            File.WriteAllText(_filePath, outputJSONObjectFileContent);
        }
        catch (System.Exception ex)
        {

            _logger.LogError($"Failure on JSON file: {_filePath}, Exception: {ex.Message}");
        }



    }

    public string GetFilePath()
    {
        return _filePath;
    }
}
