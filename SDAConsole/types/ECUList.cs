namespace SDAConsole.types
{
    public class ECUList
    {
        public string EcuAlias { get; set; } = string.Empty;
        public string EcuName { get; set; } = string.Empty;
        public string EcuAddress { get; set; } = string.Empty;
        public int EcuAddressDecimal { get; set; } = 0;
        public string LinNode { get; set; } = string.Empty;
        public int carcomPosition { get; set; } = 0;
        public List<int> TypeDesignation { get; set; } = [0];

    }

}