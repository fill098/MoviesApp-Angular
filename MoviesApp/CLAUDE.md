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
- Services references Common, Dtos, Domain and DataAccess (not Mappers). Services reach the database through repositories, never the DbContext directly. Identity work goes through `UserManager<User>`.
- Repositories (`DataAccess/Repositories`, registered in `AddDataAccess`):
  - The generic `IRepository<T>` / abstract `Repository<T>` has `GetAllAsync` (no tracking), `GetByIdAsync` (tracked), and `AddAsync`/`UpdateAsync`/`DeleteAsync`, which each call `SaveChangesAsync`. All of these methods are `virtual`.
  - Each entity has its own repository (e.g. `IDirectorRepository : IRepository<Director>`). It **overrides** the generic methods when it needs `Include`s or sorting. Don't redeclare an inherited method in the entity's interface: that's CS0108, a build error. The entity's interface only adds new methods.
  - `DirectorRepository` overrides `GetAllAsync` (includes Movies, sorted by LastName then FirstName) and `GetByIdAsync` (includes Movies, no tracking, for reading). It adds `GetByIdTrackedAsync` (includes Movies, tracked) for update and delete, and `ExistsDirectorAsync` (an `AnyAsync` check, used by `MovieService` to validate `DirectorId`).
  - `MovieRepository` overrides `GetAllAsync` (sorted by Title) and `GetByIdAsync`, both without tracking and including Director, Actors and Reviews.
  - `ActorRepository` overrides `GetAllAsync` (sorted by LastName then FirstName) and `GetByIdAsync`, both without tracking and including Movies. It adds `GetByIdTrackedAsync` (includes Movies, tracked) for update and delete.
- Mappers references Dtos and Domain.
- DataAccess references Domain.
- Dtos and Common reference no other projects. A `using MovieApp.Domain...` in a DTO file is a CS0234 build error; DTOs carry enums such as `Genre` as `string`.

The `MoviesApp/`, `MoviesApp.DataAccess/` and `MoviesApp.Domain/` folders next to the solution hold only stale `bin`/`obj` output from the old project names. They are not projects; ignore them.
- Mappers holds the `Mapster` and `Mapster.DependencyInjection` package references. Services also references `Mapster` because it injects `IMapper`.

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
- **MovieApp.Dtos**: request/response classes grouped by feature (`Auth/`, `Users/`, `Directors/`, `Actors/`, `Movies/`). `Actors/` follows the Directors pattern (`ActorReadDto` with `FullName` and `MoviesActedIn`; identical `CreateActorDto`/`UpdateActorDto`). `Movies/CreateMovieDto` takes `Genre` as a string and `DirectorId` plus `ActorIds` instead of navigation objects. `Directors/` holds `DirectorReadDto` (includes `FullName` and `MoviesDirected`, a list of titles), plus `CreateDirectorDto` and `UpdateDirectorDto`. The create and update DTOs have the same fields on purpose, with `[StringLength]` matching `DirectorConfiguration`. They have no Id, UserId or Movies. `Common/ErrorResponse` (`StatusCode`, `Message`, `Errors`, `TraceId`) is the body of **every** error response. Validation uses DataAnnotations.
- **MovieApp.Mappers** (Mapster):
  - `AddMappers()` uses `TypeAdapterConfig.GlobalSettings` with `RequireExplicitMapping = true`.
  - It scans this assembly for `IRegister` classes in `Configurations/`, compiles them at startup (a broken mapping crashes startup), and registers `IMapper` (`ServiceMapper`, scoped).
  - Every source→destination pair needs a `NewConfig<,>()` in a config class, or mapping it throws.
  - `UserMappingConfig` maps `User → UserReadDto` but ignores `Roles`, because roles come from `UserManager`.
  - `DirectorMappingConfig` maps:
    - `Director → DirectorReadDto` (`FullName`; `MoviesDirected` from `Movies`, so the movies must be `Include`d);
    - `CreateDirectorDto → Director` and `UpdateDirectorDto → Director`. Update uses `_mapper.Map(dto, trackedEntity)`, which only overwrites the DTO's fields and keeps Id, UserId, CreatedDate and Movies.
  - `ActorMappingConfig` maps `Actor → ActorReadDto` (`MoviesActedIn` from `Movies`, so they must be `Include`d) and `CreateActorDto → Actor` / `UpdateActorDto → Actor` (update maps onto the tracked entity, like Directors).
  - `MovieMappingConfig` maps `Movie → MovieReadDto` (`DirectorName`, `Actors` as full names, `AverageRating` rounded to 1 decimal and `ReviewCount`), so Director, Actors and Reviews must be `Include`d. It has no `CreateMovieDto → Movie` mapping yet, and `Genre` (string → enum) plus `ActorIds` (ids → loaded `Actor` entities) need handling in the service or the config.
  - `DirectorService`, `ActorService` and `MovieService` use `IMapper`; `AuthService` still maps by hand.
- **MovieApp.Services**:
  - `Abstractions/` holds the interfaces and `Implementations/` the classes.
  - `Models/` holds internal types such as `TokenResult(Token, ExpiresAt)`.
  - `ICurrentUser` (`Id`, `IsAdmin`) is the services' way to know who is calling. It's implemented in the API (`Security/CurrentUser`, which reads the JWT `sub` claim and throws `UnauthorizedException` when there is none). It is registered as scoped in `AddApi`.
  - `DependencyInjection.AddServices()` registers them as scoped.
  - `TokenService` creates HS256 JWTs with `JsonWebTokenHandler`, using claims `sub` (user id), `name`, `email` and one `role` claim per role.
  - `AuthService`:
    - `RegisterAsync` creates the user and adds the `Customer` role. It throws `BadRequestException` with Identity's error messages.
    - `LogInAsync` checks the username and password and throws `UnauthorizedException` with one shared message for both failures. On success it returns the token, its expiry, the username and the user's roles.
  - `DirectorService`:
    - Get all / get by id / create / update / delete. An unknown id throws `NotFoundException`.
    - Create sets `UserId` from `ICurrentUser.Id`.
    - Delete must throw `ConflictException` (409) when the director still has movies, because Director→Movies is `Restrict` and the database would otherwise fail with a 500.
  - `ActorService`: same shape as `DirectorService` (get all / get by id / create / update / delete; create sets `UserId`). Delete needs no conflict check because the `MovieActors` links cascade.
  - `MovieService`: get all / get by id. Create is in progress; it injects `IDirectorRepository` to check `DirectorId`. Validation failures must throw `BadRequestException`, not `ArgumentException` (which `GlobalExceptionHandler` turns into a 500).
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
    - `AuthController` is `[AllowAnonymous]`. It has `POST api/auth/register` (returns 201 with a `UserReadDto`) and `POST api/auth/login` (returns 200 with a `LogInUserResponseDto`). Its actions accept a `CancellationToken` but don't pass it on yet.
    - `DirectorsController`:
      - `GET api/directors` and `GET api/directors/{id:int}`: any logged-in user.
      - `POST` (201 with `CreatedAtAction(nameof(GetById))`), `PATCH {id:int}` (200 with the updated `DirectorReadDto`; it takes the full `UpdateDirectorDto`, so it behaves like a PUT) and `DELETE {id:int}` (204): `[Authorize(Roles = Roles.Admin)]` on each action, not on the controller.
      - It passes the `CancellationToken` through to the service and repository.
    - `ActorsController` has the same five endpoints as `DirectorsController` (including `PATCH {id:int}`).
    - `MoviesController` has the two GETs and an admin-only `POST`; the service side of create is in progress, and update/delete are not written yet.
    - Controllers stay thin and call a service interface. They don't check for `null`: services throw, and `GlobalExceptionHandler` turns the exception into the status code.
    - Catalogue data (directors, movies, actors) is admin-only to change. Customers add reviews.
    - 401/403 responses come from the authorization middleware, so they have no `ErrorResponse` body.

### Data model decisions (enforced in the database)

- Table names are plural; `Genre` is stored as a string; `Movie.AverageRating` and `FullName` are ignored (computed in C#). `AverageRating` loads all reviews, so compute ratings in queries for lists.
- Delete behaviour: Director→Movies `Restrict`; Movie→Reviews and both sides of the `MovieActors` join table `Cascade`; User→Reviews `Cascade`; the optional "added by" `UserId` on Movie/Director/Actor `SetNull`.
- Check constraints: `Movies.DurationMinutes > 0`, `Reviews.Rating` between `Review.MinRating` and `Review.MaxRating` (1–10). Unique index on `Reviews (UserId, MovieId)`: one review per user per movie.
- Seed data (`RoleConfiguration`, `UserConfiguration`, `UserRoleConfiguration`): `Admin` and `Customer` roles and an `admin` user in the Admin role, using fixed GUIDs and a precomputed password hash. Changing those values requires a new migration.
- Test data (`DataAccess/Seed/TestDataSeeder`, hooked up with `UseSeeding`/`UseAsyncSeeding` in `AddDataAccess`): directors, movies, actors, reviews and a `customer` user (`Customer123!`, Customer role). EF runs it on every `Update-Database`, `dotnet ef database update` and `Database.Migrate()`, but it only inserts when the `Directors` table is empty. It is not `HasData`: it needs no migration, and the database generates the ids, so later inserts never collide with seeded ids.

## Configuration

The real connection string `ConnectionStrings:MovieAppDb` and `Jwt:SecretKey` are in the API project's **user secrets** (`dotnet user-secrets list --project MovieApp.Api`). `appsettings.json` leaves both empty on purpose; it holds `Jwt:Issuer` (`MovieApp.Api`), `Jwt:Audience` (`MovieApp.Client`) and `Jwt:ExpirationInMinutes` (60). `appsettings.Development.json` has a placeholder connection string. User secrets override it in Development. Startup fails if the secret key is missing or shorter than 32 characters. The database is a local PostgreSQL 18 on port 5432.

## Working with this user

The user is learning and building the app step by step: do only the step asked for, then name the next one. When they say they want to run a command themselves (e.g. migrations), give the command instead of running it. Visual Studio may have project files open and can overwrite edits to `.csproj` files.
