using DAL.Shared.Models;

namespace SDAConsole.types.ConfigHub
{
    public class ECUListCompatibleHardware
    {

        public SoftwarePart softwarePart { get; set; } = new SoftwarePart();

        public List<CompatibilityDataSchemaHardwarePart> compatibleHardwareParts { get; set; } = new List<CompatibilityDataSchemaHardwarePart>();

        public List<CompatibilityDataSchemaHardwarePart> incompatibleHardwareParts { get; set; } = new List<CompatibilityDataSchemaHardwarePart>();

    }

    public class SoftwarePart
    {
        public string Id { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string KdpType { get; set; } = string.Empty;
        public List<VariantExpressionPartNumberDetails> VariantExpressions { get; set; } = new List<VariantExpressionPartNumberDetails>();
    }

    public class VariantExpressionPartNumberDetails
    {
        public string Id { get; set; } = string.Empty;
        // public TypeDesignation TypeDesignation { get; set; } = new TypeDesignation();
        // public List<Variant> Variants { get; set; } = new List<Variant>();
        // public ValidTime ValidTime { get; set; } = new ValidTime();
        // public ServiceTime ServiceTime { get; set; } = new ServiceTime();
        // public DevelopmentValidFrom DevelopmentValidFrom { get; set; } = new DevelopmentValidFrom();
        // public DevelopmentValidTo DevelopmentValidTo { get; set; } = new DevelopmentValidTo();
        // public string ReplacedBy { get; set; } = string.Empty;
        // public string Replaces { get; set; } = string.Empty;
        public bool IsComoExpression { get; set; } = false;
        public string Context { get; set; } = string.Empty;
        public string ecuSolutionParameter { get; set; } = string.Empty;
        public List<string> ConfigurationParameterValues { get; set; } = new List<string>();
    }

    public class CompatibilityDataSchemaHardwarePart
    {
        public List<VariantExpressionPartNumberDetails> VariantExpressions { get; set; } = new List<VariantExpressionPartNumberDetails>();
        public string PositionName { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string SxblReference { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string KdpPartType { get; set; } = string.Empty;
        public string PartType { get; set; } = string.Empty;
        public string FileURI { get; set; } = string.Empty;
        public string VbfHeaderSoftwareVersion { get; set; } = string.Empty;
        public string VbfEcuAddress { get; set; } = string.Empty;
        public int Version { get; set; } = 0;
        public string CheckSum { get; set; } = string.Empty;
        public int FileSize { get; set; } = 0;
        public bool IsEndOfLine { get; set; } = false;
        public bool IsPreStationLoaded { get; set; } = false;
        // public Project Project { get; set; } = new Project();
        // public ConfidenceLevel ConfidenceLevel { get; set; } = new ConfidenceLevel();
        public string Id { get; set; } = string.Empty;
        // public List<Validation> Validations { get; set; } = new List<Validation>();
        // public PositionName PositionName { get; set; } = new PositionName();
        // public Filestate Filestate { get; set; } = new Filestate();
        // public List<string> FilestateWarningMessages { get; set;} = new List<string>();
        // public bool Blocked {get ;set;}= false;

    }



}