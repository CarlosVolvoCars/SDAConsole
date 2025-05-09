using Microsoft.Extensions.Logging;
using DAL.Shared.Models;
using DAL.Shared;
using DAL.GetReadout;
using System.Text.Json;

namespace SDAConsole.services
{
    public class VehicleObjectService
    {
        private readonly DalConfiguration _dalConfiguration;
        private readonly ILogger _logger;

        public VehicleObjectService(DalConfiguration dalConfiguration, ILogger logger)
        {
            _dalConfiguration = dalConfiguration ?? throw new ArgumentNullException(nameof(dalConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<VehicleObject> GenerateVehicleObjectAsync(ReadoutReport instructionResult)
        {
            if (instructionResult == null)
            {
                throw new ArgumentNullException(nameof(instructionResult), "Readout report cannot be null.");
            }
            _logger.LogInformation("DAL Configuration: {0}", JsonSerializer.Serialize(_dalConfiguration, new JsonSerializerOptions { WriteIndented = true }));
            var vosClient = new VosClientFactory(_dalConfiguration, _logger).Create();
            return await vosClient.GetVehicleObject(instructionResult);
        }
    }
}
