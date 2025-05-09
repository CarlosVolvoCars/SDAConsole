using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SDAConsole.interfaces.ConfigHub;
using SDAConsole.interfaces.API;
using SDAConsole.types.ConfigHub;
using SDAConsole.types;

namespace SDAConsole.ConfigHub
{
    public class ConfigHubClient : IConfigHubClient
    {

        private readonly IConfigHubConfig configManager;
        private readonly IApiClient apiClient;
        private readonly ILogger logger;
        private readonly ILoginController loginController;
        // private List<string> baseLineId;

        public ConfigHubClient(IConfigHubConfig configManager, IApiClient apiClient, ILogger logger)
        {
            this.configManager = configManager;
            this.apiClient = apiClient;
            this.logger = logger;
            loginController = new LoginController(configManager, apiClient);

        }

        public async Task SignInAsync(string userName, string password, ServerInstanceOption server, string token)
        {
            if (String.IsNullOrEmpty(apiClient.BaseUrl))
            {
                switch (server)
                {
                    //  case ServerInstanceOption.DEV:
                    //       apiClient.BaseUrl = configManager.BaseUrl_Dev;
                    //       break;
                    //  case ServerInstanceOption.QA:
                    //       apiClient.BaseUrl = configManager.BaseUrl_QA;
                    //       break;
                        case ServerInstanceOption.PROD:
                            apiClient.BaseUrl = configManager.BaseUrl_Prod;
                            break;
                        default:
                            throw new ConfigHubException($"Not supported server choice \"{server}\" provided for login");
                }
            }

            try
            {
                apiClient.AuthorizationHeader = await loginController.Login(userName, password, token);

            }
            catch (Exception ex)
            {
                throw new ConfigHubException($"Could not log in. The Username or Password might be incorrect. Response from the server: {ex.Message}");
            }
        }

        public async Task GetBaseLineIdByName(string name)
        {
            logger.LogInformation($"Getting BaseLineId for \"{name}\" from ConfigHub API Session.");
            var baselineUrl = configManager.GetBaselineByNameUrl.Replace("<BaselineName>", name);

            var response = await apiClient.GetAsync(baselineUrl);
            var baseLines = JsonConvert.DeserializeObject<ConfigHubBaseline>(response);

            if (baseLines != null)
            {
                var baseLine = baseLines.Id;
                if (baseLine != null)
                {
                    configManager.SetBaselineMetaData(baseLines);
                    configManager.SetTargetBaselineID(baseLine);
                    logger.LogInformation($"BaseLineId for \"{name}\" is \"{baseLine}\".");
                }
                else
                {
                    logger.LogError($"BaseLine with name \"{name}\" not found.");
                    throw new ConfigHubException($"BaseLine with name \"{name}\" not found.");
                }
            }
            else
            {
                logger.LogError($"Failed to retrieve BaseLineId for \"{name}\" from the ConfigHub API Session.");
                throw new ConfigHubException("Failed to retrieve BaseLineId from the ConfigHub API Session.");
            }
        }

        internal async Task<List<ECUListApplicableSubBaseline>> GetSubBaselinesFromMetadata()
        {
            // We need to get the sub-baselines from the metadata which is stored in the configManager as BaselineMetaData
            // The metadata is a JSON string that contains the sub-baselines nested inside the "BaselineMetaData" property which has
            // a list of "ConfigHubSubBaseline" property, which has the final list of "SubBaselines" as strings, we will take each string and
            // create a new ECUListApplicableSubBaseline object with the VALUE of the SubBaselines and the SubbaselineID

            var subBaselines = configManager.BaselineMetaData.SubBaselines;
            List<ECUListApplicableSubBaseline> ecuListApplicableSubBaselines = new List<ECUListApplicableSubBaseline>();

            foreach (var subBaseline in subBaselines)
            {
                var subBaselineId = subBaseline.Handle;
                var subBaselineName = subBaseline.Name;

                if (subBaseline.SubBaselines != null)
                {
                    foreach (var subSubBaseline in subBaseline.SubBaselines)
                    {
                        var ecuListApplicableSubBaseline = new ECUListApplicableSubBaseline
                        {
                            SubBaselineID = subSubBaseline
                        };

                        ecuListApplicableSubBaselines.Add(ecuListApplicableSubBaseline);
                    }
                }
            }
            return ecuListApplicableSubBaselines;


        }

        internal async Task<List<ECUListApplicableSubBaseline>> GetCompatibleHardwareSubBaselineById(string subBaselineId, List<TestObjectConfiguration.EcuObject> ecuListData)
        {

            List<ECUListApplicableSubBaseline> ecuListApplicableSubBaselines = new List<ECUListApplicableSubBaseline>();

            var baselineUrl = configManager.GetCompatibleHardwareFromBaselineByIDUrl.Replace("<BaselineID>", subBaselineId);

            var response = await apiClient.GetAsync(baselineUrl);
            var compatibleItems = JsonConvert.DeserializeObject<List<ECUListCompatibleHardware>>(response);

            if (compatibleItems != null)
            {
                foreach (var item in compatibleItems)
                {

                    foreach (var ecuList in ecuListData) // TODO: Remove/Exclude ECU from list when found
                    {

                        foreach (var part in item.compatibleHardwareParts)
                        {
                            if (part.PartNumber.Contains(ecuList.HardwarePartNumber + ecuList.Version) && ecuList.HardwarePartNumber != "" && ecuList.Version != "" && ecuList.Ecu.Contains(part.PositionName))
                            {
                                ECUListApplicableSubBaseline bufferSubBaseline = new ECUListApplicableSubBaseline
                                {
                                    SWType = item.softwarePart.KdpType,
                                    SWPartNumber = item.softwarePart.PartNumber,
                                    EcuName = ecuList.Ecu,
                                    EcuAddress = ecuList.EcuAddress,
                                    EcuHardwarePartNumber = ecuList.HardwarePartNumber,
                                    EcuHardwarePartVersion = ecuList.Version,
                                    EcuHardwareSolutionParameter = item.softwarePart.VariantExpressions[0].ecuSolutionParameter,
                                    SubBaselineID = subBaselineId
                                };

                                // Iterate thru part.VariantExpressions and create a new List<VariantExpression>
                                foreach (var variantExpression in item.softwarePart.VariantExpressions)
                                {
                                    VariantExpression bufferVariantExpression = new VariantExpression
                                    {
                                        Comment = ecuList.Ecu,
                                        Context = variantExpression.Context,
                                        ecuSolutionParameter = variantExpression.ecuSolutionParameter,
                                        ConfigurationParameterValues = variantExpression.ConfigurationParameterValues
                                    };

                                    bufferSubBaseline.VariantExpressions.Add(bufferVariantExpression);
                                }

                                ecuListApplicableSubBaselines.Add(bufferSubBaseline);
                                break;
                            }
                        }

                    }

                }

            }
            else
            {
                throw new ConfigHubException($"Failed to retrieve Compatible Hardware for Baseline ID \"{subBaselineId}\" from the ConfigHub API Session.");
            }

            return ecuListApplicableSubBaselines;

        }

        internal async Task<List<String>> GetECUConfigurationParameterValuesBySubBaselineId(string subBaselineId, List<TestObjectConfiguration.EcuObject> ecuListData)
        {

            List<String> ecuSolutionParameterValues = new List<String>();

            var baselineUrl = configManager.GetCompatibleSoftwareFromBaselineByIDUrl.Replace("<BaselineID>", subBaselineId);

            var response = await apiClient.GetAsync(baselineUrl);
            var compatibleItems = JsonConvert.DeserializeObject<List<ECUListCompatibleSoftware>>(response);

            if (compatibleItems != null)
            {
                foreach (var item in compatibleItems)
                {
                    foreach (var ecuHWItem in ecuListData)
                    {
                        // foreach (var part in item.hardwarePart)
                        // {
                            if (item.hardwarePart.PartNumber.Contains(ecuHWItem.HardwarePartNumber + ecuHWItem.Version) && ecuHWItem.HardwarePartNumber != "" && ecuHWItem.Version != "")
                            {
                                foreach (var variantExpression in item.hardwarePart.VariantExpressions)
                                {
                                    ecuSolutionParameterValues.Add(variantExpression.ecuSolutionParameter);

                                }
                            }
                        // }
                    }
                }

            }
            else
            {
                throw new ConfigHubException($"Failed to retrieve Compatible Software for Baseline ID \"{subBaselineId}\" from the ConfigHub API Session.");
            }

            return ecuSolutionParameterValues;
        }

    }


}
