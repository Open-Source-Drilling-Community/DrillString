using OSDC.UnitConversion.DrillingRazorMudComponents;

public static class APIUtils
{
    // API parameters
    public static readonly string HostNameDrillString = NORCE.Drilling.DrillString.WebApp.Configuration.DrillStringHostURL!;
    public static readonly string HostBasePathDrillString = "DrillString/api/";
    public static readonly HttpClient HttpClientDrillString = APIUtils.SetHttpClient(HostNameDrillString, HostBasePathDrillString);
    public static readonly NORCE.Drilling.DrillString.ModelShared.Client ClientDrillString = new NORCE.Drilling.DrillString.ModelShared.Client(APIUtils.HttpClientDrillString.BaseAddress!.ToString(), APIUtils.HttpClientDrillString);
    // Field api
    public static readonly string HostDevDigiWells = "https://dev.digiwells.no/";
    public static readonly string HostBasePathField = "Field/api/";
    public static readonly HttpClient HttpClientField = APIUtils.SetHttpClient(HostDevDigiWells, HostBasePathField);
    public static readonly NORCE.Drilling.DrillString.ModelShared.Client ClientField = new NORCE.Drilling.DrillString.ModelShared.Client(APIUtils.HttpClientField.BaseAddress!.ToString(), APIUtils.HttpClientField);
    // Cluster api
    public static readonly string HostBasePathCluster = "Cluster/api/";
    public static readonly HttpClient HttpClientCluster = APIUtils.SetHttpClient(HostDevDigiWells, HostBasePathCluster);
    public static readonly NORCE.Drilling.DrillString.ModelShared.Client ClientCluster = new NORCE.Drilling.DrillString.ModelShared.Client(APIUtils.HttpClientCluster.BaseAddress!.ToString(), APIUtils.HttpClientCluster);
    // Well api
    public static readonly string HostBasePathWell = "Well/api/";
    public static readonly HttpClient HttpClientWell = APIUtils.SetHttpClient(HostDevDigiWells, HostBasePathWell);
    public static readonly NORCE.Drilling.DrillString.ModelShared.Client ClientWell = new NORCE.Drilling.DrillString.ModelShared.Client(APIUtils.HttpClientWell.BaseAddress!.ToString(), APIUtils.HttpClientWell);
    // WellBore api
    public static readonly string HostBasePathWellBore = "WellBore/api/";
    public static readonly HttpClient HttpClientWellBore = APIUtils.SetHttpClient(HostDevDigiWells, HostBasePathWellBore);
    public static readonly NORCE.Drilling.DrillString.ModelShared.Client ClientWellBore = new NORCE.Drilling.DrillString.ModelShared.Client(APIUtils.HttpClientWellBore.BaseAddress!.ToString(), APIUtils.HttpClientWellBore);

    public static readonly string HostNameUnitConversion = NORCE.Drilling.DrillString.WebApp.Configuration.UnitConversionHostURL!;
    public static readonly string HostBasePathUnitConversion = "UnitConversion/api/";

    // API utility methods
    public static HttpClient SetHttpClient(string host, string microServiceUri)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
        HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri(host + microServiceUri)
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}

public class GroundMudLineDepthReferenceSource : IGroundMudLineDepthReferenceSource
{
    public double? GroundMudLineDepthReference { get; set; } = null;
}

public class RotaryTableDepthReferenceSource : IRotaryTableDepthReferenceSource
{
    public double? RotaryTableDepthReference { get; set; } = null;
}

public class SeaWaterLevelDepthReferenceSource : ISeaWaterLevelDepthReferenceSource
{
    public double? SeaWaterLevelDepthReference { get; set; } = null;
}
public class WellHeadDepthReferenceSource : IWellHeadDepthReferenceSource
{
    public double? WellHeadDepthReference { get; set; } = null;
}
