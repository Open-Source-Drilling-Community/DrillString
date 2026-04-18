namespace NORCE.Drilling.DrillString.WebPages;

public interface IDrillStringAPIUtils
{
    string HostNameDrillString { get; }
    string HostBasePathDrillString { get; }
    HttpClient HttpClientDrillString { get; }
    NORCE.Drilling.DrillString.ModelShared.Client ClientDrillString { get; }

    string HostNameField { get; }
    string HostBasePathField { get; }
    HttpClient HttpClientField { get; }
    NORCE.Drilling.DrillString.ModelShared.Client ClientField { get; }

    string HostNameCluster { get; }
    string HostBasePathCluster { get; }
    HttpClient HttpClientCluster { get; }
    NORCE.Drilling.DrillString.ModelShared.Client ClientCluster { get; }

    string HostNameWell { get; }
    string HostBasePathWell { get; }
    HttpClient HttpClientWell { get; }
    NORCE.Drilling.DrillString.ModelShared.Client ClientWell { get; }

    string HostNameWellBore { get; }
    string HostBasePathWellBore { get; }
    HttpClient HttpClientWellBore { get; }
    NORCE.Drilling.DrillString.ModelShared.Client ClientWellBore { get; }

    string HostNameUnitConversion { get; }
    string HostBasePathUnitConversion { get; }
}
