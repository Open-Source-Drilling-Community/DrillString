# NORCE.Drilling.DrillString.WebPages

Reusable Razor class library containing the DrillString web pages and dependent editor components extracted from the `WebApp` host.

## Included UI
- `DrillStringMain`
- `DrillStringAddPanel`
- `DrillStringEditPanel`
- `DrillStringSectionAdd`
- `DrillStringSectionEditor`
- `SensorAdd`
- dependent `DrillStringComponent*` pages and shared UI helpers

## Dependencies
- `OSDC.DotnetLibraries.Drilling.WebAppUtils`
- `ModelSharedOut`
- `MudBlazor`
- `OSDC.UnitConversion.DrillingRazorMudComponents`
- `Plotly.Blazor`

## Host integration
The consuming Blazor host must:

1. Reference this package.
2. Provide an implementation of `IDrillStringWebPagesConfiguration`.
3. Register `IDrillStringWebPagesConfiguration` and `IDrillStringAPIUtils` in DI.
4. Add the library assembly to the router `AdditionalAssemblies`.

Example registration:

```csharp
builder.Services.AddSingleton<IDrillStringWebPagesConfiguration>(new WebPagesHostConfiguration
{
    DrillStringHostURL = builder.Configuration["DrillStringHostURL"] ?? string.Empty,
    FieldHostURL = builder.Configuration["FieldHostURL"] ?? string.Empty,
    ClusterHostURL = builder.Configuration["ClusterHostURL"] ?? string.Empty,
    WellHostURL = builder.Configuration["WellHostURL"] ?? string.Empty,
    WellBoreHostURL = builder.Configuration["WellBoreHostURL"] ?? string.Empty,
    UnitConversionHostURL = builder.Configuration["UnitConversionHostURL"] ?? string.Empty,
});
builder.Services.AddSingleton<IDrillStringAPIUtils, DrillStringAPIUtils>();
```
