using DAL.Shared.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using SDAConsole.types;
using SDAConsole.types.ConfigHub;
using SDAConsole.ConfigHub;
using System.Text.Json;
using ShellProgressBar;

public class InstallationReportParser
{
    private readonly InstallationReport _readoutReportFile;
    private readonly DalConfiguration _config;
    private readonly ILogger _logger;

    public InstallationReportParser(InstallationReport readoutReportFile, DalConfiguration config, ILogger logger)
    {
        _readoutReportFile = readoutReportFile;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetVehicleIdentificationNumber(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    string vin = ExtractVinFromResponse(hexResponse);
                    _logger.LogInformation($"VIN: {0}", vin);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted VIN: {vin}");

                    return vin;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }

            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return string.Empty;
        });
    }

    public async Task<string> GetVehicleType(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    string vehicleTypeCode = ExtractVehicleTypeFromResponse(hexResponse);
                    _logger.LogInformation($"VehicleTypeCode: {0}", vehicleTypeCode);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted Vehicle Type Code: {vehicleTypeCode}");

                    return vehicleTypeCode;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }

            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return string.Empty;
        });
    }

    public async Task<string> GetFactoryOrderNumber(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    string fyon = ExtractFYONFromResponse(hexResponse);
                    _logger.LogInformation($"FYON: {0}", fyon);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted FYON: {fyon}");

                    return fyon;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }

            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return string.Empty;
        });
    }

    public async Task<string> GetChassisNumber(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    string chassis = ExtractChassisFromResponse(hexResponse);
                    _logger.LogInformation("Chassis: {0}", chassis);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted Chassis: {chassis}");

                    return chassis;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");

            }

            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return string.Empty;
        });
    }

    public async Task<string> GetFactoryCode(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    string factoryCode = ExtractFactoryCodeFromResponse(hexResponse);
                    _logger.LogInformation("FactoryCode: {0}", factoryCode);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted Factory Code: {factoryCode}");

                    return factoryCode;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }
            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return string.Empty;
        });
    }

    public async Task<long> GetStructureWeek(IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22F114 Response", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F114"))
                {
                    string hexResponse = readoutSection.Response;
                    long structureWeek = ExtractStructureWeekFromResponse(hexResponse);
                    _logger.LogInformation("StructureWeek: {0}", structureWeek);
                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted Structure Week: {structureWeek}");

                    if (structureWeek != 0)
                    {
                        return structureWeek;
                    }
                    else
                    {
                        _logger.LogError("Failed to parse StructureWeek to long.");
                        return 0; // Return a default value or handle the error as needed
                    }
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }
            sub.WriteLine("DID 22F114 Response not found");
            _logger.LogError("DID 22F114 Response not found");
            return 0;
        });
    }

    public async Task<List<string>> GetConfigurationModellerExpresions(List<string> steeringCoMoExpresions, IProgressBar? parent = null)
    {
        return await Task.Run(() =>
        {
            using var sub = new ProgressHelper($"Reading: VehicleReadouts for DID 22C011 Response to retrieve CoMo expressions from HPA", _readoutReportFile.VehicleReadouts.Count, parent as ProgressBar);

            _logger.LogInformation("Starting GetConfigurationModellerExpresions method...");
            _logger.LogInformation("Steering CoMo Expresions: {0}", steeringCoMoExpresions.Count > 0 ? string.Join(", ", steeringCoMoExpresions) : "No Steering CoMo Expresions provided");

            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22C011"))
                {
                    string hexResponse = readoutSection.Response;
                    List<string> coMoExpresions = ExtractCoMoExpresionsFromResponse(hexResponse);
                    var outputCOMOsFileContent = JsonSerializer.Serialize(coMoExpresions, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText("./dataStore/COMOsBeforeSteering.json", outputCOMOsFileContent);
                    // File.WriteAllText("../../../dataStore/COMOsBeforeSteering.json", outputCOMOsFileContent);
                    _logger.LogInformation("CoMo Expresions before filtering with Steering CoMos: {0}", coMoExpresions.Count > 0 ? string.Join(", ", coMoExpresions) : "No CoMo Expresions found");

                    // Filter the CoMo expressions based on the steeringCoMoExpresions
                    // If steeringCoMoExpresions is empty, do not filter
                    if (steeringCoMoExpresions.Count > 0)
                    {
                        using var sub2 = new ProgressHelper($"Filtering CoMo Expresions with Steering CoMos", steeringCoMoExpresions.Count, sub.Parent as ProgressBar);

                        List<string> updatedCoMoExpresions = new List<string>(coMoExpresions);
                        string concatenatedSteeringCoMoExpresions = "";

                        foreach (var steeringCoMoExpression in steeringCoMoExpresions)
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
                                    // Replace the COMO value with the one from the steeringCoMoExpresions
                                    updatedCoMoExpresions[i] = $"{steeringCoMoExpression}";
                                    steeringCoMoExpressionAdded = true;
                                    // break;
                                }
                            }

                            if (!steeringCoMoExpressionAdded)
                            {
                                updatedCoMoExpresions.Add(steeringCoMoExpression);
                            }

                            concatenatedSteeringCoMoExpresions = string.Join(", ", concatenatedSteeringCoMoExpresions, steeringCoMoExpression);
                            sub2.Tick($"Steering CoMo Expression{concatenatedSteeringCoMoExpresions} added to CoMo Expresions");
                        }

                        coMoExpresions = updatedCoMoExpresions;
                    }
                    var outputCOMOsFileContentAfter = JsonSerializer.Serialize(coMoExpresions, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText("./dataStore/COMOsAfterSteering.json", outputCOMOsFileContentAfter);
                    // File.WriteAllText("../../../dataStore/COMOsAfterSteering.json", outputCOMOsFileContentAfter);
                    _logger.LogInformation("CoMo Expresions after filtering with Steering CoMos: {0}", coMoExpresions.Count > 0 ? string.Join(", ", coMoExpresions) : "No CoMo Expresions found");

                    sub.Complete($"Readout Section DID Found: {readoutSection.Request}, Extracted CoMo Expresions: {coMoExpresions.Count}");

                    return coMoExpresions;
                }
                sub.Tick($"Readout Section Request #{_readoutReportFile.VehicleReadouts.IndexOf(readoutSection) + 1} Reviewed");
            }
            sub.WriteLine("DID 22C011 Response not found");
            _logger.LogError("DID 22C011 Response not found");
            return new List<string>();
        });
    }

    public async Task<List<ECUList>> GetEcuList( List<ECUList> ecuListData)
    {
        return await Task.Run(() =>
        {
            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22C010"))
                {
                    string hexResponse = readoutSection.Response;
                    List<ECUList> matchedECUs = ExtractECUsListResponse(hexResponse, ecuListData);
                    // _logger.LogInformation("ECUs: {0}", matchedECUs);
                    return matchedECUs;
                }
            }
            return new List<ECUList>();
        });
    }

    public async Task<List<ApplDiagnosticPartNumbers>> GetApplDiagnosticPartNumberList()
    {
        return await Task.Run(() =>
        {
            List<ApplDiagnosticPartNumbers> applDiagnosticPartNumberList = new List<ApplDiagnosticPartNumbers>();
            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F120"))
                {
                    string hexResponse = readoutSection.Response;
                    int ecuAddressDecimal = readoutSection.EcuAddress;
                    applDiagnosticPartNumberList.Add(ExtractApplDiagnosticPartNumberListResponse(hexResponse, ecuAddressDecimal));



                }
            }
            // _logger.LogInformation("ApplDiagnosticPartNumber: {0}", applDiagnosticPartNumberList);
            return applDiagnosticPartNumberList;

        });
    }

    public async Task<List<HardwarePartNumbers>> GetHardwarePartNumberList()
    {
        return await Task.Run(() =>
        {
            List<HardwarePartNumbers> hardwarePartNumbersList = new List<HardwarePartNumbers>();
            foreach (VehicleReadoutResponse readoutSection in _readoutReportFile.VehicleReadouts)
            {
                if (readoutSection.Request.Contains("22F12A"))
                {
                    string hexResponse = readoutSection.Response;
                    int ecuAddressDecimal = readoutSection.EcuAddress;
                    hardwarePartNumbersList.Add(ExtractHardwarePartNumberListResponse(hexResponse, ecuAddressDecimal));



                }
            }
            // _logger.LogInformation("ApplDiagnosticPartNumber: {0}", hardwarePartNumbersList);
            return hardwarePartNumbersList;

        });
    }

    public async Task<List<ECUListApplicableSubBaseline>> GetCompatibleSoftwareFilteredByECUListFromTargetBaseline(List<TestObjectConfiguration.EcuObject> ecuListData, List<ECUListApplicableSubBaseline> baselines, ConfigHubClient configHubClient )
    {
        return await Task.Run(async () =>
        {
            List<ECUListApplicableSubBaseline> ecuListApplicableSubBaselines = new List<ECUListApplicableSubBaseline>();
            foreach (ECUListApplicableSubBaseline baseline in baselines)
            {
                List<ECUListApplicableSubBaseline> ecuListApplicableSubBaseline = await configHubClient.GetCompatibleHardwareSubBaselineById(baseline.SubBaselineID, ecuListData);

                if (ecuListApplicableSubBaseline.Count > 0)
                {
                    _logger.LogInformation("Matching HW {0} found for SubBaselineID: {0}", ecuListApplicableSubBaseline[0].EcuHardwarePartNumber, ecuListApplicableSubBaseline[0].SubBaselineID);
                    ecuListApplicableSubBaselines.AddRange(ecuListApplicableSubBaseline);
                }
                else
                {
                    _logger.LogWarning("Matching HW not found for SubBaselineID: {0}", baseline.SubBaselineID);
                }

            }
            return ecuListApplicableSubBaselines;
        });
    }

    public async Task<List<string>> GetCompatibleHardwareEcuConfigurationParametersFilterByECUListFromTargetBaselines(List<TestObjectConfiguration.EcuObject> ecuListData, List<ECUListApplicableSubBaseline> baselines, ConfigHubClient configHubClient)
    {
        return await Task.Run(async () =>
        {
            List<string> ecuConfigurationParameterValues = new List<string>();
            foreach (ECUListApplicableSubBaseline baseline in baselines)
            {
                List<string> ecuListApplicableSubBaseline = await configHubClient.GetECUConfigurationParameterValuesBySubBaselineId(baseline.SubBaselineID, ecuListData);

                if (ecuListApplicableSubBaseline.Count > 0)
                {
                    ecuConfigurationParameterValues.AddRange(ecuListApplicableSubBaseline);
                }

            }
            return ecuConfigurationParameterValues;

        });
    }

    public string ExtractVinFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract VIN (offset 16 bits + SID = 5 bytes, length 17 bytes)
            byte[] vinBytes = responseBytes.Skip(5).Take(17).ToArray();

            // Convert to ASCII
            string vin = Encoding.ASCII.GetString(vinBytes).Trim();

            return vin;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting VIN: {0}", ex.Message);
            return string.Empty;
        }
    }

    public string ExtractVehicleTypeFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract VehicleTypeCode (offset 272 bits + SID = 37 bytes, length 3 bytes)
            byte[] vehicleTypeCodeBytes = responseBytes.Skip(37).Take(3).ToArray();

            // Convert to ASCII
            string vehicleTypeCode = Encoding.ASCII.GetString(vehicleTypeCodeBytes).Trim();

            return vehicleTypeCode;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting VehicleTypeCode: {0}", ex.Message);
            return string.Empty;
        }
    }

    public string ExtractFYONFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract FYON (offset 152 bits + SID = 22 bytes, length 9 bytes)
            byte[] fyonBytes = responseBytes.Skip(22).Take(9).ToArray();

            // Convert to ASCII
            string fyon = Encoding.ASCII.GetString(fyonBytes).Trim();

            return fyon;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting FYON: {0}", ex.Message);
            return string.Empty;
        }
    }

    public string ExtractChassisFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract Chassis (offset 224 bits + SID = 31 bytes, length 6 bytes)
            byte[] chassisBytes = responseBytes.Skip(31).Take(6).ToArray();

            // Convert to ASCII
            string chassis = Encoding.ASCII.GetString(chassisBytes).Trim();

            return chassis;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting Chassis: {0}", ex.Message);
            return string.Empty;
        }
    }

    public string ExtractFactoryCodeFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract FactoryCode (offset 296 bits + SID = 40 bytes, length 2 bytes)
            byte[] factoryCodeBytes = responseBytes.Skip(40).Take(2).ToArray();

            // Convert to ASCII
            string factoryCode = Encoding.ASCII.GetString(factoryCodeBytes).Trim();

            return factoryCode;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting FactoryCode: {0}", ex.Message);
            return string.Empty;
        }
    }

    public long ExtractStructureWeekFromResponse(string hexResponse)
    {
        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Extract StructureWeek (offset 312 bits + SID = 42 bytes, length 6 bytes)
            byte[] structureWeekBytes = responseBytes.Skip(42).Take(6).ToArray();

            // Convert to ASCII
            string structureWeek = Encoding.ASCII.GetString(structureWeekBytes).Trim();
            long.TryParse(structureWeek, out long result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting StructureWeek: {0}", ex.Message);
            return 0;
        }
    }

    public List<string> ExtractCoMoExpresionsFromResponse(string hexResponse)
    {
        try
        {
            var index = 0;
            var numberOfCoMoExpresions = 0;

            List<byte[]> coMoExpresionsBytesArray = new List<byte[]>();
            List<string> coMoExpresionsArray = new List<string>();

            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Remove the first 6 bytes (0x00 0x00 0x00 0x00 0x00 0x00)
            responseBytes = responseBytes.Skip(6).ToArray();

            // Iterate until the Respoonse of the next 8 bytes is 0x00 after an ascii char (2C = COMMA
            while (index < responseBytes.Length - 8)
            {
                if (responseBytes[index] == 0x2C && responseBytes[index + 8] == 0x00)
                {
                    break;
                }

                if (responseBytes[index] == 0x2C)
                {
                    index++;
                    continue;
                }
                else
                {
                    coMoExpresionsBytesArray.Add(responseBytes.Skip(index).Take(8).ToArray());
                    index += 8;
                    numberOfCoMoExpresions++;
                }
            }

            // Convert to ASCII
            for (int i = 0; i < numberOfCoMoExpresions; i++)
            {
                coMoExpresionsArray.Add(Encoding.ASCII.GetString(coMoExpresionsBytesArray[i]).Trim().Replace(":", "-"));
            }

            return coMoExpresionsArray;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting CoMo Expresions: {0}", ex.Message);
            return new List<string>();
        }
    }

    public List<ECUList> ExtractECUsListResponse(string hexResponse, List<ECUList> ecuListData)
    {
        var ecuCarComIndex = 0;
        var ecuCarComOffset = 2;
        List<ECUList> mountedECUsArray = new List<ECUList>();

        try
        {
            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Remove the first 5 bytes (0x62 0xC0 0x10 0x00 0x00)
            responseBytes = responseBytes.Skip(5).ToArray();

            // Iterate through the response bytes to Match only mounted ECUs
            ecuCarComIndex = ecuCarComOffset;
            for (int i = 0; i < responseBytes.Length; i++) // TODO: Check if this is the correct length (iN CARCONFIG This DID has a reserved area of in the last byte)
            {
                ecuCarComIndex = ecuCarComOffset + (i + 1); // +1 because CarComIndex starts at 1

                if (responseBytes[i] == 2) // ECO is Mounted
                {
                    foreach (var ecuItem in ecuListData)
                    {
                        if (ecuItem.carcomPosition == ecuCarComIndex)
                        {
                            mountedECUsArray.Add(ecuItem);
                            break;
                        }
                    }
                }

            }

            return mountedECUsArray;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting ECUs: {0}", ex.Message);
            return new List<ECUList>();
        }
    }

    public ApplDiagnosticPartNumbers ExtractApplDiagnosticPartNumberListResponse(string hexResponse, int ecuAddressDecimal)
    {
        try
        {
            ApplDiagnosticPartNumbers applDiagnosticPartNumber = new ApplDiagnosticPartNumbers();

            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Skip the first 3 bytes (0x62 0xF1 0x20)
            responseBytes = responseBytes.Skip(3).ToArray();

            if (responseBytes.Length > 0)
            {
                // Take the first 4 bytes and convert to ASCII
                string partNumber = BitConverter.ToString(responseBytes.Take(4).ToArray()).Replace("-", "");

                // Take the next 2 bytes and convert to ASCII
                string asciiPart = Encoding.ASCII.GetString(responseBytes.Skip(4).Take(3).ToArray());

                // Combine the two parts
                applDiagnosticPartNumber.EcuAddressDecimal = ecuAddressDecimal;
                applDiagnosticPartNumber.ApplDiagnosticPartNumber = $"{partNumber}{asciiPart}";
            }
            else
            {
                applDiagnosticPartNumber.EcuAddressDecimal = ecuAddressDecimal;
                applDiagnosticPartNumber.ApplDiagnosticPartNumber = hexResponse;
            }

            return applDiagnosticPartNumber;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting Appl Diagnostic Part Number List: {0}", ex.Message);
            return new ApplDiagnosticPartNumbers();
        }
    }

    public HardwarePartNumbers ExtractHardwarePartNumberListResponse(string hexResponse, int ecuAddressDecimal)
    {
        try
        {
            HardwarePartNumbers hardwarePartNumber = new HardwarePartNumbers();

            // Convert hex string to bytes
            byte[] responseBytes = Enumerable.Range(0, hexResponse.Length / 2)
                                             .Select(i => Convert.ToByte(hexResponse.Substring(i * 2, 2), 16))
                                             .ToArray();

            // Skip the first 3 bytes (0x62 0xF1 0x20)
            responseBytes = responseBytes.Skip(3).ToArray();

            if (responseBytes.Length > 0)
            {
                // Take the first 4 bytes and convert to ASCII
                string partNumber = BitConverter.ToString(responseBytes.Take(4).ToArray()).Replace("-", "");

                // Take the next 3 bytes and convert to ASCII
                string asciiPart = Encoding.ASCII.GetString(responseBytes.Skip(4).Take(3).ToArray()).Trim();

                // Combine the two parts
                hardwarePartNumber.EcuAddressDecimal = ecuAddressDecimal;
                hardwarePartNumber.HardwarePartNumber = $"{partNumber}";
                hardwarePartNumber.Version = $"{asciiPart}";
            }
            else
            {
                hardwarePartNumber.EcuAddressDecimal = ecuAddressDecimal;
                hardwarePartNumber.HardwarePartNumber = hexResponse;
                hardwarePartNumber.Version = "";
            }

            return hardwarePartNumber;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error extracting Hardware Part Number List: {0}", ex.Message);
            return new HardwarePartNumbers();
        }
    }
}