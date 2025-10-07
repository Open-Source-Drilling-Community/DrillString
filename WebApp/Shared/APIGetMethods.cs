using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

public enum TypeSelect {WellBore, Field};
public class GetDigiWellClassClient
{
    private static readonly string HostNameDigiWells = "https://dev.digiwells.no/";
    //private string HostBasePathFields = "Field/api/Fields/";
    private HttpClient HttpClientField;
    private TypeSelect loadingType;
    public string NameForAuxiliarText { get; }
    public async Task GetAllAsync(Dictionary<Guid, string>? nameIdPairs)
    {
        try
        {
            using HttpResponseMessage response = await HttpClientField.GetAsync("");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            List<Guid>? Ids = JsonSerializer.Deserialize<List<Guid>?>(jsonResponse);
            List<string> JsonFiles = new();
            
            if (Ids != null)
            {
                foreach (Guid id in Ids)
                {
                    using HttpResponseMessage dataResponse = await HttpClientField.GetAsync(id.ToString());
                    string? jsonData = await dataResponse.Content.ReadAsStringAsync();
                    switch (loadingType)
                    {
                        case TypeSelect.Field:
                        {
                            NORCE.Drilling.DrillString.ModelShared.Field? data = JsonSerializer.Deserialize<NORCE.Drilling.DrillString.ModelShared.Field?>(jsonData);
                                try
                                {
                                    if (!nameIdPairs.ContainsKey(id))
                                    {
                                        JsonFiles.Add(data.Name);
                                        nameIdPairs.Add(id, data.Name);
                                    }                         
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex.ToString() + "\t Impossible to get field name!");
                                }
                            break;
                        }
                        case TypeSelect.WellBore:
                        {
                            NORCE.Drilling.DrillString.ModelShared.WellBore? data = JsonSerializer.Deserialize<NORCE.Drilling.DrillString.ModelShared.WellBore?>(jsonData);
                            try
                            {
                                if (!nameIdPairs.ContainsKey(id))
                                {
                                    JsonFiles.Add(data.Name);
                                    nameIdPairs.Add(id, data.Name);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.ToString() + "\t Impossible to get wellbore name!");
                            }
                            break;
                        }
                        default:
                        {
                            Console.Write("\tField or WellBore not found!");
                            break;
                        }
                     }
                }
            }
            else
            {
                nameIdPairs = null;
            }
  
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString() + "\t Impossible to load from client");
        }
    }
    public string CheckGuidExistance(Guid? idToCheck, Dictionary<Guid, string>? idToNameDictExtracted)
    {
        if (idToCheck == null)
        {
            return $"No {NameForAuxiliarText} assigned";     
        }
        if (idToNameDictExtracted == null)
        {
            return $"No {NameForAuxiliarText} found on the {NameForAuxiliarText} microservice";     
        }
        if (idToNameDictExtracted.ContainsKey(idToCheck.Value))
        {
            if (idToNameDictExtracted[idToCheck.Value] != null )
            {
                return idToNameDictExtracted[idToCheck.Value];
            }
            else
            {
                return "";                    
            }
        }
        else
        {
            return "Name unavailable";
        }
    }
    
    public GetDigiWellClassClient(TypeSelect microserviceName)
    {
        string HostBasePathFields = $"{microserviceName}/api/{microserviceName}/";
        this.NameForAuxiliarText = microserviceName.ToString();
        this.HttpClientField = new HttpClient()
        {
            BaseAddress = new Uri(HostNameDigiWells + HostBasePathFields),
        };
    }
}
