using DAL.Shared.Models;

namespace SDAConsole.types.ConfigHub

{
    public class ECUListCompatibleSoftware
    {

        public HardwarePart hardwarePart { get; set; } = new HardwarePart();

        public List<CompatibilityDataSchemaSoftwarePart> compatibleSoftwareParts { get; set; } = new List<CompatibilityDataSchemaSoftwarePart>();

        public List<CompatibilityDataSchemaSoftwarePart> incompatibleSoftwareParts { get; set; } = new List<CompatibilityDataSchemaSoftwarePart>();

    }

    public class HardwarePart
    {
        public string Id { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string KdpType { get; set; } = string.Empty;
        public List<VariantExpressionPartNumberDetails> VariantExpressions { get; set; } = new List<VariantExpressionPartNumberDetails>();
    }

    public class CompatibilityDataSchemaSoftwarePart
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
        public long? FileSize { get; set; } = 0;
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