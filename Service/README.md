# DrillString Service

The `Service` project hosts the ASP.NET Core microservice that exposes DrillString domain operations over HTTP. It fronts the SQLite-backed data store and publishes the OpenAPI contract consumed by downstream clients.

## Responsibilities
- Hosts controllers for `DrillString` and `DrillStringComponent`, offering CRUD endpoints as well as light/heavy data listings backed by `DrillStringManager` and `DrillStringComponentManager`.
- Persists data in `home/DrillString.db` through the singleton `SqlConnectionManager`, which creates, validates, backs up, and indexes the SQLite schema at startup.
- Enforces consistent JSON serialization via `JsonSettings` (string enums, casing preserved) so API payloads match the types defined in `Model`.
- Serves a merged OpenAPI document through `SwaggerMiddlewareExtensions`. The middleware rewrites server URLs on the fly to support reverse proxies and hosts Swagger UI under the `/DrillString/api` path base.

## How other projects use the service
- `Service/Service.csproj` references `..\\Model\\Model.csproj`, so all API contracts are typed with the classes defined in the Model project.
- Building the service in `Debug` runs the `CreateSwaggerJson` MSBuild target, exporting the runtime schema to `ModelSharedOut/json-schemas/DrillStringFullName.json`.
- `ModelSharedOut` consumes those schemas, merges them with any external dependencies, and writes the merged result back to `Service/wwwroot/json-schema/DrillStringMergedModel.json` alongside the generated client (`ModelSharedOut/DrillStringMergedModel.cs`).
- `ServiceTest` hits the live endpoints to validate behaviour, relying on the schema generated above to keep DTOs in sync.
- `WebApp` and any external applications reference `ModelSharedOut` to obtain the same types the service exposes, ensuring UI payloads align with the REST contract.

## Running locally
- Seed or inspect the SQLite database under `home/DrillString.db`; it is created automatically when the service starts.
- Restore dependencies and build with `dotnet build Service.csproj`. The Debug build generates the latest OpenAPI snapshot for `ModelSharedOut`.
- Launch the API with `dotnet run --project Service.csproj`. The service listens under `/DrillString/api`, while Swagger UI is available at `/DrillString/api/swagger`.
- After modifying the `Model` project, rebuild this service and run `dotnet run --project ..\\ModelSharedOut` to regenerate the merged OpenAPI bundle and shared C# client.

## Deployment notes
- The container image published to Docker Hub is `digiwells/norcedrillingdrillstringservice`. Production and staging instances are reachable at `https://app.digiwells.no/DrillString/api` and `https://dev.digiwells.no/DrillString/api` respectively.
- The microservice is typically deployed with Kubernetes and Helm charts maintained in the main `DrillingAndWells` repository.
