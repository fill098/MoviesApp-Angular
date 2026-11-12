# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Core (.NET 10) Web API for a movies app, backed by PostgreSQL via EF Core and ASP.NET Identity. An Angular front end is planned but not in this repository yet. There are no test projects yet.

## Commands

Run from the solution folder (the one containing `MoviesApp.slnx`).

```bash
dotnet build MoviesApp.slnx
dotnet run --project MoviesApp                      # opens Scalar at /scalar (http://localhost:5174, https://localhost:7215)

# EF Core migrations: DbContext lives in DataAccess, connection string comes from the API project
dotnet ef migrations add <Name> --project MoviesApp.DataAccess --startup-project MoviesApp --output-dir Migrations
dotnet ef database update      --project MoviesApp.DataAccess --startup-project MoviesApp
dotnet ef migrations has-pending-model-changes --project MoviesApp.DataAccess --startup-project MoviesApp
```

The user also runs migrations from Visual Studio's Package Manager Console (`Add-Migration` / `Update-Database`, startup project = `MoviesApp`, default project = `MoviesApp.DataAccess`).

## Build rules that bite

- `Directory.Build.props` sets `TargetFramework`, `Nullable`, `ImplicitUsings`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild` and **`TreatWarningsAsErrors=true` for every configuration** (its comment claiming Release-only is out of date). Any analyzer or `.editorconfig` style warning fails the build, so don't repeat these properties in `.csproj` files.
- **Central Package Management** (`Directory.Packages.props`): `PackageReference` items must not have a `Version` attribute (NU1008). Keep all `Microsoft.EntityFrameworkCore.*` versions identical, or NU1605 downgrade errors appear.
- `.editorconfig` enforces file-scoped namespaces, `using`s outside the namespace, `_camelCase` private fields and PascalCase constants as warnings (so errors). `Migrations/*.cs` are marked `generated_code = true` so EF's generated code passes analyzers.

## Architecture

Three projects, references point one way: `MoviesApp` (API) → `MoviesApp.DataAccess` → `MoviesApp.Domain`; the API also references Domain directly.

- **MoviesApp.Domain**: entities (`Movie`, `Director`, `Actor`, `Review`, `User : IdentityUser`), the `Genre` enum and `Roles` constants. No EF Core attributes; all mapping lives in DataAccess. Entities derive from `BaseEntity` (`int Id`, `CreatedDate`, `UpdatedDate`); `User` does not (string Identity key).
- **MoviesApp.DataAccess**:
  - `Context/MoviesAppDbContext` derives from `IdentityDbContext<User>`, applies every `IEntityTypeConfiguration` in the assembly via `ApplyConfigurationsFromAssembly`, and sets `UpdatedDate` on modified `BaseEntity` rows in `SaveChanges`/`SaveChangesAsync`.
  - `Configurations/` holds one class per entity. Each relationship is configured once, on the side that holds the foreign key (e.g. Movie↔Actor and Movie→Reviews are in `MovieConfiguration`); don't configure it again from the other side.
  - `DependencyInjection.AddDataAccess(IConfiguration)` registers the DbContext (Npgsql) and `AddIdentityCore<User>().AddRoles<IdentityRole>()`. It throws at startup if the `MoviesAppDb` connection string is empty.
- **MoviesApp** (API): `Program.cs` calls `AddDataAccess`, controllers, OpenAPI, and Scalar (Development only). `Controllers/` is empty so far.

### Data model decisions (enforced in the database)

- Table names are plural; `Genre` is stored as a string; `Movie.AverageRating` and `FullName` are ignored (computed in C#). `AverageRating` loads all reviews, so compute ratings in queries for lists.
- Delete behaviour: Director→Movies `Restrict`; Movie→Reviews and both sides of the `MovieActors` join table `Cascade`; User→Reviews `Cascade`; the optional "added by" `UserId` on Movie/Director/Actor `SetNull`.
- Check constraints: `Movies.DurationMinutes > 0`, `Reviews.Rating` between `Review.MinRating` and `Review.MaxRating` (1–10). Unique index on `Reviews (UserId, MovieId)`: one review per user per movie.
- Seed data (`RoleConfiguration`, `UserConfiguration`, `UserRoleConfiguration`): `Admin` and `Customer` roles and an `admin` user in the Admin role, using fixed GUIDs and a precomputed password hash. Changing those values requires a new migration.

## Configuration

The connection string `ConnectionStrings:MoviesAppDb` is in the API project's **user secrets** (`dotnet user-secrets list --project MoviesApp`), along with `Jwt:SecretKey` for the planned JWT authentication. `appsettings.json` leaves it empty on purpose. The database is a local PostgreSQL 18 on port 5432.

## Working with this user

The user is learning and building the app step by step: do only the step asked for, then name the next one. When they say they want to run a command themselves (e.g. migrations), give the command instead of running it. Visual Studio may have project files open and can overwrite edits to `.csproj` files.

The empty `MovisApp.Domain/` folder is a leftover from renaming the project to `MoviesApp.Domain` and can be ignored.
