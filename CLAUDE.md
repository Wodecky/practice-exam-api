# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

`practice-exam-api` is an ASP.NET Core Web API targeting **.NET 10**, organized with
**Clean Architecture**. The solution file is `PracticeExam.slnx` (the XML solution
format — there is no `.sln`).

## Commands

All commands run from the repository root.

```bash
dotnet build                                  # build the whole solution
dotnet run --project src/PracticeExam.Api      # run the API (http://localhost:5284, https://localhost:7280)
dotnet test                                    # run all tests
dotnet watch --project src/PracticeExam.Api    # run the API with hot reload
```

Run one test project, or a single test / subset by filter:

```bash
dotnet test tests/PracticeExam.Domain.UnitTests          # one project only
dotnet test --filter "FullyQualifiedName~ClassName.MethodName"
dotnet test --filter "FullyQualifiedName~PracticeExam.Domain.UnitTests.SomeNamespace"
```

In development, the OpenAPI document is served at `/openapi/v1.json`.

## Architecture

Four layers under `src/`, plus tests under `tests/`. The **dependency rule** points
inward — outer layers reference inner ones, never the reverse:

```
Api ──▶ Infrastructure ──▶ Application ──▶ Domain
 └──────────────────────────▶ Application
```

- **PracticeExam.Domain** — entities, value objects, domain logic. Has **no project
  references** and no framework dependencies beyond the BCL. `Common/Entity.cs` is
  the base type for entities (identity-based, `Guid` key).
- **PracticeExam.Application** — use cases, DTOs, and the *abstractions*
  (`Common/Interfaces`, `Abstractions`) that infrastructure implements. References
  Domain only.
- **PracticeExam.Infrastructure** — concrete implementations of Application
  abstractions: persistence (`Persistence/`), external services. References
  Application.
- **PracticeExam.Api** — HTTP entry point: controllers, middleware, and the
  composition root (`Program.cs`). References Application and Infrastructure.

### Composition

Each non-API layer exposes a `DependencyInjection` static class with an extension
method that registers its own services. `Program.cs` wires them up:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
```

When adding a service, register it in the `DependencyInjection` class of the layer
that *owns the implementation* — do not register infrastructure types from the API
project.

### Where things go

- A new abstraction (interface) → `Application` (`Common/Interfaces` or `Abstractions`).
- Its implementation (DB, HTTP client, etc.) → `Infrastructure`, registered in
  `Infrastructure/DependencyInjection.cs`.
- A new domain concept → `Domain/Entities` or `Domain/Common`; keep it free of
  framework and persistence concerns.
- A new endpoint → a controller in `Api/Controllers`, delegating to Application
  use cases.

## Database

The API reads from an **externally-owned SQLite database** — `practice_exam.db` in
the sibling `practice-exam-db/` repo, whose schema is migrated by **sqitch**, not by
EF Core.

- EF Core is used **read-only**. Do **not** add EF migrations or let EF create or
  alter the schema — sqitch owns it. `PracticeExamDbContext` maps the existing
  snake_case tables explicitly in `OnModelCreating`.
- The connection string is `ConnectionStrings:PracticeExamDb`.
- Row ids are stored as lowercase 32-char hex (`lower(hex(randomblob(16)))`), not
  the canonical dashed Guid format. Entity `Guid` keys therefore need a value
  converter (`v => v.ToString("N")` ↔ `Guid.Parse`) — see `Exam`'s mapping in
  `PracticeExamDbContext`.
- `Infrastructure.UnitTests` exercise the mapping against an in-memory SQLite
  database; `Api.IntegrationTests` run against a seeded throwaway file. Both
  hand-roll the table DDL — keep it in sync with the sqitch schema.

## Tests

Each production project has a matching test project under `tests/`. Put a test in
the project that matches the layer under test:

- `PracticeExam.Domain.UnitTests` — domain entities and logic.
- `PracticeExam.Application.UnitTests` — use cases (mock Application abstractions).
- `PracticeExam.Infrastructure.UnitTests` — infrastructure implementations.
- `PracticeExam.Api.IntegrationTests` — end-to-end HTTP tests that boot the real
  app via `WebApplicationFactory<Program>` (`Program` is exposed as a partial
  class in `Program.cs` for this purpose).

## Continuous Integration

`.github/workflows/ci.yml` runs `restore` → `build` (Release) → `dotnet test`
on every PR to `main` and on pushes to `main`. The job is named **Build & Test**.

A repository ruleset on `main` requires PRs and a passing **Build & Test**
status check before merging (admins can bypass). The required-check context is
matched by the job's `name:` string — if you rename the CI job, update the
ruleset's required status check to match, or the gate silently stops enforcing.
