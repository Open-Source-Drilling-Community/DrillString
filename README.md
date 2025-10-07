# DrillString Solution

The DrillString solution models the composition of a drill string and exposes it through a microservice, shared client library, and web user interface. It targets .NET 8 and is built and deployed as a set of cooperating projects.

## Repository layout
| Project | Type | Purpose |
| --- | --- | --- |
| `Model/` | Class library | Domain model describing drill strings, sections, components, parts, and component types. |
| `Service/` | ASP.NET Core | REST microservice that persists drill-string data in SQLite and publishes an OpenAPI contract. |
| `ModelSharedOut/` | Console app | Generates merged OpenAPI schemas and a strongly-typed C# client from the service contract. |
| `WebApp/` | Blazor Server | MudBlazor-based UI for browsing and editing drill strings via the generated client. |
| `ModelTest/` | NUnit tests | Validates domain calculations and defaulting logic in `Model`. |
| `ServiceTest/` | NUnit tests | Exercises the HTTP API exposed by `Service` using generated DTOs. |
| `home/` | Data folder | Holds the `DrillString.db` SQLite file created by the microservice at runtime. |

Supporting assets include Docker workflows under `.github/workflows`, solution metadata in `DrillString.sln`, and documentation tooling (`Model/docfx.json`).

## Data flow & dependencies
1. The **Model** project defines the domain classes and is referenced directly by the **Service** project. Controllers serialize these types using custom JSON settings to keep property names and enum values stable.
2. When the **Service** builds in `Debug`, the `CreateSwaggerJson` MSBuild target exports its OpenAPI description to `ModelSharedOut/json-schemas`.
3. Running the **ModelSharedOut** console app merges that schema (plus any additional dependencies) and regenerates:
   - `ModelSharedOut/DrillStringMergedModel.cs` (C# client and DTOs).
   - `Service/wwwroot/json-schema/DrillStringMergedModel.json` (served through Swagger).
4. The **WebApp** and **ServiceTest** projects reference `ModelSharedOut` so that UI pages and automated tests use the same types the service publishes.
5. Data is persisted in `home/DrillString.db` via the singleton `SqlConnectionManager`. The manager validates schema consistency and creates timestamped backups before destructive changes.

## Local development workflow
1. **Restore & build**: `dotnet build DrillString.sln` (requires .NET SDK 8.0 or later).  
2. **Run the service**: `dotnet run --project Service/Service.csproj`. The API listens under `/DrillString/api`; Swagger UI is available at `/DrillString/api/swagger`.  
3. **Generate shared model** (when the API contract changes):  
   ```powershell
   dotnet run --project ModelSharedOut/ModelSharedOut.csproj
   ```  
   This refreshes the shared client and copies the merged schema back into the service.
4. **Run the web app** (optional): `dotnet run --project WebApp/WebApp.csproj`. The app hosts at `/DrillString/webapp` and requires the service endpoint URLs to be reachable (configured through `appsettings.*`).  
5. **Execute tests**:  
   ```powershell
   dotnet test ModelTest/ModelTest.csproj
   dotnet test ServiceTest/ServiceTest.csproj
   ```
6. **Documentation**: From `Model/`, run `docfx docfx.json` to generate API docs into `_site/` (DocFX CLI required).  
7. **Database management**: Inspect or reset `home/DrillString.db` as needed; the service recreates tables on startup if the file is missing.

## Configuration & environment
- Environment-specific service URLs are resolved at runtime from configuration files (`Service/appsettings*.json`, `WebApp/appsettings*.json`).  
- `Program.cs` in the service and web app adds `UsePathBase` so that reverse proxies (Kubernetes ingress) can mount the components under `/DrillString/api` and `/DrillString/webapp`.  
- Certificates are bypassed in development for external dependencies (see `APIUtils.SetHttpClient`). Ensure this is hardened before production use outside the controlled Digiwells environment.

## Deployment overview
- **Containers**: Dockerfiles for `Service` and `WebApp` produce images published under the `digiwells` organization on Docker Hub (for example, `digiwells/norcedrillingdrillstringservice` and `digiwells/norcedrillingdrillstringwebappclient`).  
- **Helm/Kubernetes**: Deployment manifests reside in the main NORCE `DrillingAndWells` repository. They configure ingress paths, environment variables (service endpoints), and persistent storage mapping for `home/`.  
- **Hosted endpoints**:  
  - API: `https://dev.digiwells.no/DrillString/api/DrillString`, `https://app.digiwells.no/DrillString/api/DrillString`  
  - Swagger: `https://dev.digiwells.no/DrillString/api/swagger`, `https://app.digiwells.no/DrillString/api/swagger`  
  - Web UI: `https://dev.digiwells.no/DrillString/webapp/DrillString`, `https://app.digiwells.no/DrillString/webapp/DrillString`

## Contributing
- Keep model changes accompanied by updated tests in `ModelTest`.  
- When you touch the API surface, rerun the service build and regenerate `ModelSharedOut` so clients stay in sync.  
- Avoid committing generated binaries or `_site` outputs; only the regenerated schema (`Service/wwwroot/json-schema/DrillStringMergedModel.json`) and shared client (`ModelSharedOut/DrillStringMergedModel.cs`) are tracked.  
- For database-breaking changes, update `SqlConnectionManager` to preserve or migrate existing data and document the process in this README.

## Funding & acknowledgements
The DrillString work is funded by the [Research Council of Norway](https://www.forskningsradet.no/) and [industry partners](https://www.digiwells.no/about/board/) through the SFI DigiWells centre (2020–2028). Core contributors include Eric Cayeux, Gilles Pelfrene, and Lucas Volpi from the NORCE Energy Modelling and Automation group.
