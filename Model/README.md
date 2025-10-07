# DrillString Model

The `Model` project defines the core domain objects that describe a drill string within the NORCE drilling portfolio. These classes are shared across the solution to keep the API, automated clients, and UI components aligned on the same business vocabulary.

## Contents
- `DrillString`, `DrillStringSection`, `DrillStringComponent`, and `DrillStringComponentPart` capture the hierarchical structure of a drill string down to individual parts.
- The `DrillStringComponentTypes` namespace enumerates supported component categories (for example `DrillPipe`, `DrillCollar`, `Stabilizer`) and stores type-specific data holders.
- Domain classes default to industry-standard physical properties (density, Young modulus, heat capacity, etc.) and rely on `OSDC.DotnetLibraries` packages for drilling properties and metadata management.
- `docfx.json` is pre-configured to generate API documentation for the model if you run `docfx docfx.json` from this folder.

## How other projects use this model
- `Service/Service.csproj` references `Model` directly; controllers and managers expose these types in the REST API and populate them from storage.
- Building the service in `Debug` triggers the `CreateSwaggerJson` MSBuild target, which serializes the API (including the `Model` types) to `Service/wwwroot/json-schema/DrillStringMergedModel.json`. That schema is the authoritative contract for downstream clients.
- `ModelTest` consumes the project for unit tests that verify default values, geometry calculations, and serialization behavior.
- `ModelSharedOut` runs as a console utility that bundles the OpenAPI schema (generated from the service build) and emits a strongly typed client library (`DrillStringMergedModel.cs`). The resulting code mirrors the classes defined here so external consumers stay in sync.
- `ServiceTest` and `WebApp` reference `ModelSharedOut`; changes made in `Model` propagate to those projects once you rebuild the service and rerun the shared-model generator.

## Working with the project
- Build locally with `dotnet build Model.csproj` to validate that changes compile.
- After editing the model, rebuild `Service` and execute `dotnet run --project ..\ModelSharedOut` to refresh the generated OpenAPI bundle and shared client classes.
- Keep the JSON schema in `Service/wwwroot/json-schema` under source control so that contract changes are visible in reviews.
- When adding new entities or properties, update or add corresponding tests in `ModelTest` to cover geometry or defaulting logic.
