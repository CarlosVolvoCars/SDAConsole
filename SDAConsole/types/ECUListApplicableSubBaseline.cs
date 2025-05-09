namespace SDAConsole.types.ConfigHub
{
    public class ECUListApplicableSubBaseline
    {
        public string SWType { get; set; } = string.Empty;
        public string SWPartNumber { get; set; } = string.Empty;
        public string EcuName { get; set; } = string.Empty;
        public string EcuAddress { get; set; } = string.Empty;
        public string EcuHardwarePartNumber { get; set; } = string.Empty;
        public string EcuHardwarePartVersion { get; set; } = string.Empty;
        public string EcuHardwareSolutionParameter { get; set; } = string.Empty;
        public string SubBaselineID { get; set; } = string.Empty;
        public List<VariantExpression> VariantExpressions { get; set; } = new List<VariantExpression>();

    }

}

public class VariantExpression
    {
        public string Comment { get; set; } = string.Empty;

        public string Context { get; set; } = string.Empty;

        public string ecuSolutionParameter { get; set; } = string.Empty;

        public List<string> ConfigurationParameterValues { get; set; } = new List<string>();
    }