using OSDC.DotnetLibraries.Drilling.WebAppUtils;

namespace NORCE.Drilling.DrillString.WebPages;

public interface IDrillStringWebPagesConfiguration :
    IFieldHostURL,
    IClusterHostURL,
    IWellHostURL,
    IWellBoreHostURL,
    IDrillStringHostURL,
    IUnitConversionHostURL
{
}
