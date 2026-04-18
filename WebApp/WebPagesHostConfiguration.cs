using NORCE.Drilling.DrillString.WebPages;

namespace NORCE.Drilling.DrillString.WebApp;

public class WebPagesHostConfiguration : IDrillStringWebPagesConfiguration
{
    public string? FieldHostURL { get; set; } = string.Empty;
    public string? ClusterHostURL { get; set; } = string.Empty;
    public string? WellHostURL { get; set; } = string.Empty;
    public string? WellBoreHostURL { get; set; } = string.Empty;
    public string? DrillStringHostURL { get; set; } = string.Empty;
    public string? UnitConversionHostURL { get; set; } = string.Empty;
}
