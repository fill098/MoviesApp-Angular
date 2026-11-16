# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (.NET 10) Web API for a movies app, backed by PostgreSQL via EF Core and ASP.NET Identity. An Angular front end is planned but not in this repository yet. There are no test projects yet.

## Commands

Run from the solution folder (the one containing `MovieApp.slnx`).

```bash
dotnet build MovieApp.slnx
dotnet run --project MovieApp.Api                   # opens Scalar at /scalar (http://localhost:5174, https://localhost:7215)

# EF Core migrations: DbContext lives in DataAccess, connection string comes from the API project
dotnet ef migrations add <Name> --project MovieApp.DataAccess --startup-project MovieApp.Api --output-dir Migrations
dotnet ef database update      --project MovieApp.DataAccess --startup-project MovieApp.Api
dotnet ef migrations has-pending-model-changes --project MovieApp.DataAccess --startup-project MovieApp.Api
```

The user also runs migrations from Visual Studio's Package Manager Console (`Add-Migration` / `Update-Database`, startup project = `MovieApp.Api`, default project = `MovieApp.DataAccess`).

## Build rules that bite

- `Directory.Build.props` sets `TargetFramework`, `Nullable`, `ImplicitUsings`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild` and **`TreatWarningsAsErrors=true` for every configuration** (its comment claiming Release-only is out of date). Any analyzer or `.editorconfig` style warning fails the build, so don't repeat these properties in `.csproj` files.
- **Central Package Management** (`Directory.Packages.props`): `PackageReference` items must not have a `Version` attribute (NU1008). Keep all `Microsoft.EntityFrameworkCore.*` versions identical, or NU1605 downgrade errors appear.
- `.editorconfig` enforces file-scoped namespaces, `using`s outside the namespace, `_camelCase` private fields and PascalCase constants as warnings (so errors). `Migrations/*.cs` are marked `generated_code = true` so EF's generated code passes analyzers.
- `Nullable` is on, so nullable-reference warnings (e.g. CS8604 when passing a `User?` from `FindByNameAsync` into `CheckPasswordAsync`) are build errors. Null-check first.
- `GenerateDocumentationFile` is on (so IDE0005 "unnecessary using" is reported and fails the build); CS1591 is suppressed, so XML comments are optional.

## Architecture

### Projects and naming

Every project uses the `MovieApp.` prefix (no "s"): the folder, the `.csproj`, the root namespace and the solution `MovieApp.slnx` all share one name. Don't reintroduce the old `MoviesApp.` prefix. Only a few names keep it on purpose, because renaming them changes the database or configuration: the outer repo folder, the `MoviesAppDbContext` class and `MoviesApp.http`.

| Folder | Project file | Namespace |
|---|---|---|
| `MovieApp.Api/` | `MovieApp.Api.csproj` | `MovieApp.Api` |
| `MovieApp.DataAccess/` | `MovieApp.DataAccess.csproj` | `MovieApp.DataAccess` |
| `MovieApp.Domain/` | `MovieApp.Domain.csproj` | `MovieApp.Domain` |
| `MovieApp.Services/` | `MovieApp.Services.csproj` | `MovieApp.Services` |
| `MovieApp.Dtos/` | `MovieApp.Dtos.csproj` | `MovieApp.Dtos` |
| `MovieApp.Common/` | `MovieApp.Common.csproj` | `MovieApp.Common` |
| `MovieApp.Mappers/` | `MovieApp.Mappers.csproj` | `MovieApp.Mappers` |

Project references:
- The API references Common, Dtos, Mappers, Services and DataAccess. It reaches Domain through those projects.
- Services references Common, Dtos and Domain, **but not DataAccess or Mappers**. Services use `UserManager<User>` (from `Microsoft.Extensions.Identity.Stores` in Domain). They don't use the DbContext directly.
- Mappers references Dtos and Domain.
- DataAccess references Domain.
- Dtos and Common reference no other projects.
- Mappers holds the `Mapster` and `Mapster.DependencyInjection` package references. If Services ever injects `IMapper`, it needs Mapster too.

The new `.csproj` files (Common, Dtos, Mappers, Services) still repeat `TargetFramework`/`Nullable`/`ImplicitUsings` from the template. That's harmless, but leave those properties out of new projects.

### Layers

- **MovieApp.Domain**: entities (`Movie`, `Director`, `Actor`, `Review`, `User : IdentityUser`), the `Genre` enum and `Roles` constants (file `Constants/Rols.cs`, class `Roles`). No EF Core attributes; all mapping lives in DataAccess. Entities derive from `BaseEntity` (`int Id`, `CreatedDate`, `UpdatedDate`); `User` does not (string Identity key).
- **MovieApp.DataAccess**:
  - `Context/MoviesAppDbContext` derives from `IdentityDbContext<User>`, applies every `IEntityTypeConfiguration` in the assembly via `ApplyConfigurationsFromAssembly`, and sets `UpdatedDate` on modified `BaseEntity` rows in `SaveChanges`/`SaveChangesAsync`.
  - `Configurations/` holds one class per entity. Each relationship is configured once, on the side that holds the foreign key (e.g. Movie↔Actor and Movie→Reviews are in `MovieConfiguration`); don't configure it again from the other side.
  - `DependencyInjection.AddDataAccess(IConfiguration)` registers the DbContext (Npgsql) and `AddIdentityCore<User>(RequireUniqueEmail).AddRoles<IdentityRole>().AddEntityFrameworkStores<MoviesAppDbContext>()`. It throws at startup if the `MovieAppDb` connection string is empty.
- **MovieApp.Common**:
  - `Settings/JwtSettings` (section `Jwt`; `Issuer`, `Audience`, `SecretKey` with at least 32 chars, `ExpirationInMinutes`; constant `RoleClaimType = "role"`).
  - `Exceptions/` holds the app's exception types. The folder was renamed, but the files still declare the old misspelled namespace `MovieApp.Common.Extensinons`, so `using` statements must match the namespace, not the folder. The types are: abstract `AppException`, plus `BadRequestException` (with an `Errors` list), `UnauthorizedException`, `ForbiddenException`, `NotFoundException` and `ConflictException`. Services throw these, and the API's `GlobalExceptionHandler` turns them into HTTP status codes.
- **MovieApp.Dtos**: request/response classes grouped by feature (`Auth/`, `Users/`). `Common/ErrorResponse` (`StatusCode`, `Message`, `Errors`, `TraceId`) is the body of **every** error response. Validation uses DataAnnotations.
- **MovieApp.Mappers** (Mapster):
  - `AddMappers()` uses `TypeAdapterConfig.GlobalSettings` with `RequireExplicitMapping = true`.
  - It scans this assembly for `IRegister` classes in `Configurations/`, compiles them at startup (a broken mapping crashes startup), and registers `IMapper` (`ServiceMapper`, scoped).
  - Every source→destination pair needs a `NewConfig<,>()` in a config class, or mapping it throws.
  - `UserMappingConfig` maps `User → UserReadDto` but ignores `Roles`, because roles come from `UserManager`.
  - `MovieMappingConfig` is empty.
  - No service uses `IMapper` yet; `AuthService` still maps by hand.
- **MovieApp.Services**:
  - `Abstractions/` holds the interfaces and `Implementations/` the classes.
  - `Models/` holds internal types such as `TokenResult(Token, ExpiresAt)`.
  - `ICurrentUser` (`Id`, `IsAdmin`) is the services' way to know who is calling. It's implemented in the API (`Security/CurrentUser`, which reads the JWT `sub` claim and throws `UnauthorizedException` when there is none). **Its DI registration in `AddApi` is still commented out.**
  - `DependencyInjection.AddServices()` registers them as scoped.
  - `TokenService` creates HS256 JWTs with `JsonWebTokenHandler`, using claims `sub` (user id), `name`, `email` and one `role` claim per role.
  - `AuthService`:
    - `RegisterAsync` creates the user and adds the `Customer` role. It throws `BadRequestException` with Identity's error messages.
    - `LogInAsync` checks the username and password and throws `UnauthorizedException` with one shared message for both failures. On success it returns the token, its expiry, the username and the user's roles.
- **MovieApp.Api** (API):
  - `Program.cs` chains `AddDataAccess` → `AddMappers` → `AddServices` → `AddApi`, then `UseAuthentication` / `UseAuthorization`. Every `Add*` method in the chain must return `IServiceCollection`.
  - `Extensions/ServiceCollectionExtensions.AddApi()` sets up the API project:
    - Controllers, with `InvalidModelStateResponseFactory` returning an `ErrorResponse` instead of ProblemDetails.
    - OpenAPI, lowercase URLs, `GlobalExceptionHandler`, `IHttpContextAccessor` and JWT authentication.
  - `ExceptionHandling/GlobalExceptionHandler` (`IExceptionHandler`):
    - Maps `BadRequest`/`Unauthorized`/`Forbidden`/`NotFound`/`Conflict` exceptions to 400/401/403/404/409.
    - Anything else becomes a 500 with a generic message (logging is still a `// log` placeholder).
    - `Program.cs` calls `app.UseExceptionHandler(_ => { })` first in the pipeline. The empty lambda is required: without it (or `AddProblemDetails()`), `UseExceptionHandler()` throws at startup.
  - `Extensions/AuthenticationExtensions`:
    - Binds and validates `JwtSettings` on startup.
    - Configures JwtBearer with `MapInboundClaims = false` (claim names stay `sub`/`name`/`role`, matching `TokenService`).
    - **Sets a fallback policy that requires an authenticated user on every endpoint**, so public endpoints (register, login) need `[AllowAnonymous]`.
  - `Extensions/OpenApiExtensions` and `OpenApi/BearerSecuritySchemeTransformer` serve `/openapi/v1.json` and Scalar at `/scalar` (Development only, anonymous). They add a Bearer auth field to the docs.
  - `Controllers/`:
    - `ApiControllerBase` (`[ApiController]`, route `api/[controller]`, JSON) is the base for all controllers.
    - `AuthController` is `[AllowAnonymous]`. It has `POST api/auth/register` (returns 201 with a `UserReadDto`) and `POST api/auth/login` (returns 200 with a `LogInUserResponseDto`). Controllers stay thin and call a service interface.
    - The actions accept a `CancellationToken`, but don't pass it to the services yet.

### Data model decisions (enforced in the database)

- Table names are plural; `Genre` is stored as a string; `Movie.AverageRating` and `FullName` are ignored (computed in C#). `AverageRating` loads all reviews, so compute ratings in queries for lists.
- Delete behaviour: Director→Movies `Restrict`; Movie→Reviews and both sides of the `MovieActors` join table `Cascade`; User→Reviews `Cascade`; the optional "added by" `UserId` on Movie/Director/Actor `SetNull`.
- Check constraints: `Movies.DurationMinutes > 0`, `Reviews.Rating` between `Review.MinRating` and `Review.MaxRating` (1–10). Unique index on `Reviews (UserId, MovieId)`: one review per user per movie.
- Seed data (`RoleConfiguration`, `UserConfiguration`, `UserRoleConfiguration`): `Admin` and `Customer` roles and an `admin` user in the Admin role, using fixed GUIDs and a precomputed password hash. Changing those values requires a new migration.

## Configuration

The real connection string `ConnectionStrings:MovieAppDb` and `Jwt:SecretKey` are in the API project's **user secrets** (`dotnet user-secrets list --project MovieApp.Api`). `appsettings.json` leaves both empty on purpose; it holds `Jwt:Issuer` (`MovieApp.Api`), `Jwt:Audience` (`MovieApp.Client`) and `Jwt:ExpirationInMinutes` (60). `appsettings.Development.json` has a placeholder connection string. User secrets override it in Development. Startup fails if the secret key is missing or shorter than 32 characters. The database is a local PostgreSQL 18 on port 5432.

## Working with this user

The user is learning and building the app step by step: do only the step asked for, then name the next one. When they say they want to run a command themselves (e.g. migrations), give the command instead of running it. Visual Studio may have project files open and can overwrite edits to `.csproj` files.
