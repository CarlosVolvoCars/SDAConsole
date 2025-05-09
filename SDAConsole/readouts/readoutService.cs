using DAL.Connection.Interfaces;
using Microsoft.Extensions.Logging;
using DAL.Shared.Models;
using System.Text.Json;
using DAL.Diagnostics;
using DAL.Diagnostics.Models;
using SDAConsole.types;
using SDAConsole.helpers;

public class ReadoutService
{
    private readonly IVehicleConnection _connection;
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    private readonly DalConfiguration _config;

    public ReadoutService(IVehicleConnection connection, DalConfiguration config, Microsoft.Extensions.Logging.ILogger logger)
    {
        _connection = connection;
        _logger = logger;
        _config = config;

    }

    public async Task<(ReadoutReport Report, List<UdsResponse> Response)> GetReadoutReportFromSingleDiagRequest(string requestToSent, ushort targetAddress)
    {
        return await RetryHelper.ExecuteWithRetryAsync(async () =>
            {
                _connection.Open();
                _logger.LogTrace("Vehicle connection OPEN...");

                        _logger.LogInformation($"Connection: {JsonSerializer.Serialize(_connection, new JsonSerializerOptions { WriteIndented = true })}");


                _logger.LogTrace("UDS connection INIT...");
                var readoutService = new DiagnosticsServiceFactory(_config, _logger).Create();
                readoutService.OpenUdsConnection(_connection, _config.Diagnostics.UdsOptions);
                _logger.LogTrace("UDS connection OPEN...");

                var udsRequest = new UdsRequest
                {
                    EcuAddress = targetAddress, // Functional request
                    Request = requestToSent
                };

                _logger.LogTrace($"Attempting Functional Vehicle READOUT {udsRequest.Request}...");
                var responses = readoutService.SendRequest(udsRequest);

                _logger.LogTrace($"Functional Vehicle READOUT {udsRequest.Request} DONE...");

                readoutService.CloseUdsConnection();
                _logger.LogTrace("UDS connection CLOSED...");

                if (responses == null || responses.Count == 0)
                {
                    throw new Exception("No response from ECU.");
                }

                ReadoutReport report = new ReadoutReport();

                return (Report: report, Response: responses);
            });
    }

    public async Task<List<ApplDiagnosticPartNumbers>> AssertApplDiagnosticPartNumberListCompletenessAgainstECUList (List<ECUList> ecuList, List<ApplDiagnosticPartNumbers> applDiagnosticPartNumberList, InstallationReportParser parser)
    {
       foreach (var ecu in ecuList)
            {
                var ecuAddress = ecu.EcuAddressDecimal;
                var ecuAddressHex = ecu.EcuAddress;
                var ecuAlias = ecu.EcuAlias;

                var applDiagnosticPartNumber = applDiagnosticPartNumberList.FirstOrDefault(x => x.EcuAddressDecimal == ecuAddress);
                if (applDiagnosticPartNumber == null)
                {
                    _logger.LogWarning($"ECU {ecuAlias} not found in ApplDiagnosticPartNumberList.");

                    if (ushort.TryParse(ecuAddressHex.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out ushort ecuAliasAsUshort))
                    {
                        var readoutServiceResult2 = await this.GetReadoutReportFromSingleDiagRequest("22F120", ecuAliasAsUshort);
                        var readoutResponse2 = readoutServiceResult2.Response;

                        if (readoutResponse2[0].NoResponse != true)
                        {
                            if (applDiagnosticPartNumberList.Find( partNumberItem => partNumberItem.EcuAddressDecimal == readoutResponse2[0].EcuAddress) == null)
                            {
                                _logger.LogInformation($"ECU {ecuAlias} not found in ApplDiagnosticPartNumberList. Adding it now.");
                                applDiagnosticPartNumberList.Add(parser.ExtractApplDiagnosticPartNumberListResponse(readoutResponse2[0].Data, readoutResponse2[0].EcuAddress));

                            }

                        }

                        var jsonString2 = JsonSerializer.Serialize(readoutResponse2, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText($"./dataStore/vehicleReadout22F120{ecuAlias}.json", jsonString2);
                        // File.WriteAllText($"../../../dataStore/vehicleReadout22F120{ecuAlias}.json", jsonString2);

                    }
                    else
                    {
                        _logger.LogWarning($"Failed to parse ECU alias '{ecuAlias}' to ushort.");
                        continue;
                    }

                }
            }

        return applDiagnosticPartNumberList;
    }

    public async Task<List<HardwarePartNumbers>> AssertHardwarePartNumberListCompletenessAgainstECUList (List<ECUList> ecuList, List<HardwarePartNumbers> hardwarePartNumberList, InstallationReportParser parser)
    {
        foreach (var ecu in ecuList)
        {
            var ecuAddress = ecu.EcuAddressDecimal;
            var ecuAddressHex = ecu.EcuAddress;
            var ecuAlias = ecu.EcuAlias;

            var hardwarePartNumber = hardwarePartNumberList.FirstOrDefault(x => x.EcuAddressDecimal == ecuAddress);
            if (hardwarePartNumber == null)
            {
                _logger.LogWarning($"ECU {ecuAlias} not found in HardwarePartNumberList.");

                if (ushort.TryParse(ecuAddressHex.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out ushort ecuAliasAsUshort))
                {
                    var readoutServiceResult2 = await this.GetReadoutReportFromSingleDiagRequest("22F12A", ecuAliasAsUshort);
                    var readoutResponse2 = readoutServiceResult2.Response;

                    if (readoutResponse2[0].NoResponse != true)
                    {
                        if (hardwarePartNumberList.Find( partNumberItem => partNumberItem.EcuAddressDecimal == readoutResponse2[0].EcuAddress) == null)
                        {
                            _logger.LogInformation($"ECU {ecuAlias} not found in HardwarePartNumberList. Adding it now.");
                            hardwarePartNumberList.Add(parser.ExtractHardwarePartNumberListResponse(readoutResponse2[0].Data, readoutResponse2[0].EcuAddress));

                        }

                    }

                    var jsonString2 = JsonSerializer.Serialize(readoutResponse2, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText($"./dataStore/vehicleReadout22F12A{ecuAlias}.json", jsonString2);
                    // File.WriteAllText($"../../../dataStore/vehicleReadout22F12A{ecuAlias}.json", jsonString2);

                }
                else
                {
                    _logger.LogWarning($"Failed to parse ECU alias '{ecuAlias}' to ushort.");
                    continue;
                }

            }
        }

        return hardwarePartNumberList;
    }


}