using OSDC.UnitConversion.DrillingRazorMudComponents;

public static class DataUtils
{
    // default values
    public const double DEFAULT_VALUE = 999.25;
    public static string DEFAULT_NAME_DrillString = "Default DrillString Name";
    public static string DEFAULT_DESCR_DrillString = "Default DrillString Description";
    public static string DEFAULT_NAME_DrillStringComponent = "Default DrillStringComponent Name";
    public static string DEFAULT_DESCR_DrillStringComponent = "Default DrillStringComponent Description";

    public static string FLOATING_COLOUR = "rgba(67, 49, 228, 0.86)";
    public static string FLOATING_POSITION = "absolute; top: 25%; left: 20%; width: 75%";



    // unit management
    public static class UnitAndReferenceParameters
    {
        public static string? UnitSystemName { get; set; } = "Metric";
        public static string? DepthReferenceName { get; set; }
        public static string? PositionReferenceName { get; set; }
        public static string? AzimuthReferenceName { get; set; }
        public static string? PressureReferenceName { get; set; }
        public static string? DateReferenceName { get; set; }
        public static GroundMudLineDepthReferenceSource GroundMudLineDepthReferenceSource { get; set; } = new GroundMudLineDepthReferenceSource();
        public static RotaryTableDepthReferenceSource RotaryTableDepthReferenceSource { get; set; } = new RotaryTableDepthReferenceSource();
        public static SeaWaterLevelDepthReferenceSource SeaWaterLevelDepthReferenceSource { get; set; } = new SeaWaterLevelDepthReferenceSource();
        public static WellHeadDepthReferenceSource WellHeadDepthReferenceSource { get; set; } = new WellHeadDepthReferenceSource();
    }

    public static void UpdateUnitSystemName(string val)
    {
        UnitAndReferenceParameters.UnitSystemName = (string)val;
    }

    // units and labels
    public static readonly string DrillStringDrillStringComponentListLabel = "DrillStringComponentList";
    public static readonly string DrillStringOutputParamLabel = "DrillStringOutputParam";
    public static readonly string DrillStringNameLabel = "DrillString name";
    public static readonly string DrillStringDescrLabel = "DrillString description";
    public static readonly string DrillStringOutputParamQty = "DepthDrilling";

    public static readonly string DrillStringComponentNameLabel = "DrillStringComponent name";
    public static readonly string DrillStringComponentParamLabel = "DrillStringComponentParam";
    public static readonly string DrillStringComponentParamQty = "DepthDrilling";

    public static readonly string DrillStringComponentTypesLabel = "DrillStringComponent type";
    public static readonly string DrillPipeLabel = "DrillPipe name";
    public static readonly string DrillPipeParamLabel = "DrillPipeParam";
    public static readonly string DrillPipeParamQty = "DepthDrilling";
    public static readonly string DrillCollarLabel = "DrillCollar name";
    public static readonly string DrillCollarParamLabel = "DrillCollarParam";
    public static readonly string DrillCollarParamQty = "DepthDrilling";

    public static readonly string InputXValuesTitle = "X value";
    public static readonly string InputXValuesQty = "DepthDrilling";
    public static readonly string InputYValuesTitle = "Y value";
    public static readonly string InputYValuesQty = "Length";
    public static readonly string OutputXValuesTitle = "X value";
    public static readonly string OutputXValuesQty = "DepthDrilling";
    public static readonly string OutputYValuesTitle = "Y value";
    public static readonly string OutputYValuesQty = "Length";

}