using MudBlazor;
using MudBlazor.Services;
using NORCE.Drilling.DrillString.WebApp;
using NORCE.Drilling.DrillString.WebPages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

var webPagesConfiguration = new WebPagesHostConfiguration
{
    FieldHostURL = builder.Configuration["FieldHostURL"] ?? string.Empty,
    ClusterHostURL = builder.Configuration["ClusterHostURL"] ?? string.Empty,
    WellHostURL = builder.Configuration["WellHostURL"] ?? string.Empty,
    WellBoreHostURL = builder.Configuration["WellBoreHostURL"] ?? string.Empty,
    DrillStringHostURL = builder.Configuration["DrillStringHostURL"] ?? string.Empty,
    UnitConversionHostURL = builder.Configuration["UnitConversionHostURL"] ?? string.Empty
};

builder.Services.AddSingleton<IDrillStringWebPagesConfiguration>(webPagesConfiguration);
builder.Services.AddSingleton<IDrillStringAPIUtils, DrillStringAPIUtils>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UsePathBase("/DrillString/webapp");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
