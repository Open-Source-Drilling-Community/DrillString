using OSDC.DotnetLibraries.Drilling.WebAppUtils;

namespace NORCE.Drilling.DrillString.WebPages;

public class DrillStringAPIUtils : APIUtils, IDrillStringAPIUtils
{
    public DrillStringAPIUtils(IDrillStringWebPagesConfiguration configuration)
    {
        HostNameDrillString = Require(configuration.DrillStringHostURL, nameof(configuration.DrillStringHostURL));
        HostBasePathDrillString = "DrillString/api/";
        HttpClientDrillString = SetHttpClient(HostNameDrillString, HostBasePathDrillString);
        ClientDrillString = new NORCE.Drilling.DrillString.ModelShared.Client(HttpClientDrillString.BaseAddress!.ToString(), HttpClientDrillString);

        HostNameField = Require(configuration.FieldHostURL, nameof(configuration.FieldHostURL));
        HostBasePathField = "Field/api/";
        HttpClientField = SetHttpClient(HostNameField, HostBasePathField);
        ClientField = new NORCE.Drilling.DrillString.ModelShared.Client(HttpClientField.BaseAddress!.ToString(), HttpClientField);

        HostNameCluster = Require(configuration.ClusterHostURL, nameof(configuration.ClusterHostURL));
        HostBasePathCluster = "Cluster/api/";
        HttpClientCluster = SetHttpClient(HostNameCluster, HostBasePathCluster);
        ClientCluster = new NORCE.Drilling.DrillString.ModelShared.Client(HttpClientCluster.BaseAddress!.ToString(), HttpClientCluster);

        HostNameWell = Require(configuration.WellHostURL, nameof(configuration.WellHostURL));
        HostBasePathWell = "Well/api/";
        HttpClientWell = SetHttpClient(HostNameWell, HostBasePathWell);
        ClientWell = new NORCE.Drilling.DrillString.ModelShared.Client(HttpClientWell.BaseAddress!.ToString(), HttpClientWell);

        HostNameWellBore = Require(configuration.WellBoreHostURL, nameof(configuration.WellBoreHostURL));
        HostBasePathWellBore = "WellBore/api/";
        HttpClientWellBore = SetHttpClient(HostNameWellBore, HostBasePathWellBore);
        ClientWellBore = new NORCE.Drilling.DrillString.ModelShared.Client(HttpClientWellBore.BaseAddress!.ToString(), HttpClientWellBore);

        HostNameUnitConversion = Require(configuration.UnitConversionHostURL, nameof(configuration.UnitConversionHostURL));
        HostBasePathUnitConversion = "UnitConversion/api/";
    }

    public string HostNameDrillString { get; }
    public string HostBasePathDrillString { get; }
    public HttpClient HttpClientDrillString { get; }
    public NORCE.Drilling.DrillString.ModelShared.Client ClientDrillString { get; }

    public string HostNameField { get; }
    public string HostBasePathField { get; }
    public HttpClient HttpClientField { get; }
    public NORCE.Drilling.DrillString.ModelShared.Client ClientField { get; }

    public string HostNameCluster { get; }
    public string HostBasePathCluster { get; }
    public HttpClient HttpClientCluster { get; }
    public NORCE.Drilling.DrillString.ModelShared.Client ClientCluster { get; }

    public string HostNameWell { get; }
    public string HostBasePathWell { get; }
    public HttpClient HttpClientWell { get; }
    public NORCE.Drilling.DrillString.ModelShared.Client ClientWell { get; }

    public string HostNameWellBore { get; }
    public string HostBasePathWellBore { get; }
    public HttpClient HttpClientWellBore { get; }
    public NORCE.Drilling.DrillString.ModelShared.Client ClientWellBore { get; }

    public string HostNameUnitConversion { get; }
    public string HostBasePathUnitConversion { get; }

    private static string Require(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required host URL configuration: {propertyName}");
        }

        return value;
    }
}
