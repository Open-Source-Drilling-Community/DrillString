# DrillString WebApp

The `WebApp` project is a Blazor Server application that provides the interactive front end for the DrillString microservice. It offers CRUD flows for drill strings and components, charts, and unit conversion helpers wrapped in a MudBlazor UI.

## Responsibilities
- Hosts Razor pages and components under `Pages/DrillStringPages` and `Pages/DrillStringComponentPages` to browse, create, edit, and delete drill string entities via dialogs (`Components/DialogDeleteTemplate.razor`, etc.).
- Uses `Shared/APIUtils.cs` to configure HTTP clients and generated DTO clients (`ModelSharedOut`) that point to the DrillString microservice and related data services (Field, Cluster, Well, WellBore, UnitConversion).
- Centralizes UI state helpers in `Shared/DataUtils.cs` and renders a MudBlazor layout (`Shared/MainLayout.razor`) with navigation managed by `Shared/NavMenu.razor`.
- Pulls host URLs and dependent service endpoints from configuration (`appsettings.*`), allowing environment-specific routing when deployed behind Kubernetes ingress (matches `UsePathBase("/DrillString/webapp")` in `Program.cs`).

## How the project depends on the rest of the solution
- `WebApp/WebApp.csproj` references `..\\ModelSharedOut\\ModelSharedOut.csproj`. These generated classes are derived from the service’s merged OpenAPI schema, so the UI remains synchronized with the REST contract exposed by the `Service` project.
- `APIUtils` builds typed clients from `ModelSharedOut`, meaning any model changes must be regenerated via `ModelSharedOut` after the service build (see workflow below).
- The UI calls into the DrillString microservice (`Service`) for persistence, while auxiliary calls (Field, Cluster, Well, WellBore, UnitConversion) rely on the same client library to stay consistent.
- Tests in `ServiceTest` verify server behaviour; once the service contract passes those tests and `ModelSharedOut` is regenerated, the web application can consume the updated DTOs without manual adjustments.

## Running locally
- Restore and build the project with `dotnet build WebApp.csproj`, then start it with `dotnet run --project WebApp.csproj`. The app hosts at `https://localhost:xxxx/DrillString/webapp` (matching the configured path base).
- Ensure the DrillString service is running locally—or update the base URLs in `appsettings.Development.json`—so the embedded HTTP clients can reach their endpoints.
- When modifying the domain model or API:
  1. Update the `Model` project and rebuild the `Service` project (Debug configuration) to export the latest schema.
  2. Run `dotnet run --project ..\\ModelSharedOut\\ModelSharedOut.csproj` to refresh generated DTOs and copy `DrillStringMergedModel.json` back into the service’s `wwwroot/json-schema` folder.
  3. Rebuild the web app to pick up regenerated types.
- The SQLite database lives under `home/DrillString.db` (maintained by the service). Use the UI or API to seed data during development.

## Deployment notes
- The application container published on Docker Hub is `digiwells/norcedrillingdrillstringwebappclient`.
- Production and staging deployments are accessible at `https://app.digiwells.no/DrillString/webapp/DrillString` and `https://dev.digiwells.no/DrillString/webapp/DrillString` respectively.
- Kubernetes Helm charts in the wider `DrillingAndWells` repository manage ingress configuration, environment variables, and reverse-proxy headers consumed by `Program.cs`.
