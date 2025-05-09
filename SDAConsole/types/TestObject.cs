using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SDAConsole.types.ConfigurationModeller;

namespace SDAConsole.types;

public class TestObjectConfiguration
{
    public string ReferenceBaselineURL { get; set; }
    public string ReferenceBaselineName { get; set; }

    public int TypeDesignation { get; set; }

    public string Vin { get; set; }

    public string Fyon { get; set; }

    public string ProductType { get; set; }

    public string TypeCode { get; set; }

    public List<EcuObject> EcuList { get; set; }

    public List<ExecEnvObject> ExecEnvList { get; set; }

    public string[] Vdns { get; set; }

    public bool AllVariantExpression { get; set; }

    public bool IsCompleteCar { get; set; }

    public bool IsCompleteCPVMatch { get; set; }

    public List<CoMoExpression> CoMoExpressions { get; set; }

    public class EcuObject
    {
        public string Ecu { get; set; }

        public string EcuAddress { get; set; }

        public int EcuAddressDecimal { get; set; }

        public string ApplDiagnosticPartNumber { get; set; }

        public string ApplDiagnosticPartNumberDefault { get; set; }

        public string DefaultPinCode { get; set; }

        public SecurityCode[] SecurityCodes { get; set; }

        public SoftwarePartNumberObject[] SoftwarePartNumbers { get; set; }

        public string[] SlaveNodes { get; set; }

        public string HardwarePartNumber { get; set; }

        public string HardwareSerialNumber { get; set; }

        public string Version { get; set; }

        public string EcuStatus { get; set; }

        public class SecurityCode
        {
            public string securityArea { get; set; }

            public string code { get; set; }
        }

        public class SoftwarePartNumberObject
        {
            public string SoftwarePartNumber { get; set; }

            public string SoftwarePartType { get; set; }

            public string Version { get; set; }
        }
    }

    public class ExecEnvObject
    {
        public string ExecEnv { get; set; }
    }

    public class CoMoExpression
    {
        public string Comment { get; set; }

        public string Context { get; set; }

        public string ecuSolutionParameter { get; set; }

        public List<string> ConfigurationParameterValues { get; set; }
    }

    // Public function to erase all existing CoMoExpressions in CoMoExpressions array
    public void ClearCoMoExpressions()
    {
        CoMoExpressions.Clear();
    }

    // Public function to add a new CoMoExpression object to the CoMoExpressions array
    public void AddCoMoExpression(string comment, string context, string ecuSolutionParameter, List<string> configurationParameterValues)
    {
        var comoExpression = new CoMoExpression
        {
            Comment = comment,
            Context = context,
            ecuSolutionParameter = ecuSolutionParameter,
            ConfigurationParameterValues = configurationParameterValues
        };

        // Add the new CoMoExpression object to the CoMoExpressions array
        CoMoExpressions.Add(comoExpression);
    }

    public void AddAllVariantExpressionsToTestObject(Result result)
    {
        if (result == null)
            return;

        foreach (var ecu in result.Solved)
        {

            foreach (var variant in ecu.VariantExpressions)
            {
                CoMoExpressions.Add(new CoMoExpression
                {
                    Comment = variant.Comment + "-" + ecu.SWType + "-" + ecu.SWPartNumber,
                    Context = variant.Context,
                    ecuSolutionParameter = variant.ecuSolutionParameter,
                    ConfigurationParameterValues = variant.ConfigurationParameterValues
                });
            }
        }
    }

    // Public function to add como expresions from a List<string> to CoMoExpressions.ConfigurationParameterValues by creading a new CoMoExpression object and add it to the array CoMoExpression[] CoMoExpressions
    public void AddCoMoExpressionsFromList(string comment, string context, string ecuSolutionParameter, List<string> configurationParameterValues)
    {
        // Iterate over the configurationParameterValues list and add each value to each new created object
        foreach (var value in configurationParameterValues)
        {
            var comoExpression = new CoMoExpression
            {
                Comment = comment,
                Context = context,
                ecuSolutionParameter = ecuSolutionParameter,
                ConfigurationParameterValues = new List<string> { value }
            };

            // Add the new CoMoExpression object to the CoMoExpressions array

            CoMoExpressions.Add(comoExpression);

        }
    }

    // Public function to add ECUs from a List<ECUList> to EcuList by creating a new EcuObject object and add it to the array EcuObject[] EcuList
    public void AddEcusFromList(List<ECUList> ecuList, List<ApplDiagnosticPartNumbers> applDiagnosticPartNumberList, List<HardwarePartNumbers> hardwarePartNumberList, string excludedECUListFilePath)
    {

        List<EcuObject> ExcludedEcus = new List<EcuObject>();

        foreach (var ecuItem in ecuList)
        {
            // var tempStoredApplDiagnosticPartNumber = applDiagnosticPartNumberList.Where(diagnosticPartNumber =>
            //     diagnosticPartNumber.EcuAddressDecimal == ecuItem.EcuAddressDecimal);

            var tempStoredApplDiagnosticPartNumber = applDiagnosticPartNumberList.Where(diagnosticPartNumber => ecuItem.EcuAddressDecimal == diagnosticPartNumber.EcuAddressDecimal);

            var tempStoredHardwarePartNumber = hardwarePartNumberList.Where(hardwarePartNumber => ecuItem.EcuAddressDecimal == hardwarePartNumber.EcuAddressDecimal);

            var ecuObject = new EcuObject
            {
                Ecu = ecuItem.EcuAlias,
                EcuAddress = ecuItem.EcuAddress,
                EcuAddressDecimal = ecuItem.EcuAddressDecimal,
                ApplDiagnosticPartNumber = tempStoredApplDiagnosticPartNumber.FirstOrDefault()?.ApplDiagnosticPartNumber ?? string.Empty,
                ApplDiagnosticPartNumberDefault = "",
                DefaultPinCode = "",
                SecurityCodes = new EcuObject.SecurityCode[0],
                SoftwarePartNumbers = new EcuObject.SoftwarePartNumberObject[0],
                SlaveNodes = new string[0],
                HardwarePartNumber = tempStoredHardwarePartNumber.FirstOrDefault()?.HardwarePartNumber ?? string.Empty,
                HardwareSerialNumber = "",
                Version = tempStoredHardwarePartNumber.FirstOrDefault()?.Version ?? string.Empty,
                EcuStatus = ""
            };

            // Add the new EcuObject object to the EcuList array only:
            // if it is not already present or
            // if the tempStoredHardwarePartNumber is not null or empty
            // else add to another list for later output file generation
            if (EcuList.Any(existingEcu => existingEcu.EcuAddressDecimal == ecuObject.EcuAddressDecimal) || !string.IsNullOrEmpty(tempStoredHardwarePartNumber.FirstOrDefault()?.HardwarePartNumber))
            {
                EcuList.Add(ecuObject);
            }
            else
            {
                ExcludedEcus.Add(ecuObject);
            }

        }

        // Output the excluded ECUs to a file (Handles the case where the content is null or empty)
        if (ExcludedEcus.Count > 0)
        {
            var excludedEcusFileContent = JsonConvert.SerializeObject(ExcludedEcus, Formatting.Indented);
            File.WriteAllText(excludedECUListFilePath, excludedEcusFileContent);
        }
    }

    public void AddECUObjectToList(EcuObject ecuObject)
    {
        EcuList.Add(ecuObject);
    }
}