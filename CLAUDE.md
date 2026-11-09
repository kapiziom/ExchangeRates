# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

.NET 10 ASP.NET Core MVC app that shows currency exchange rates (base currency PLN) fetched in the background from the NBP API (`https://api.nbp.pl/api/exchangerates/tables/a`). It uses an EF Core **InMemory** database, so there is no DB setup or migrations; data is lost on restart and is repopulated by the background service.

## Commands

Solution: `ExchangeRates.sln`.

```
dotnet build
dotnet run --project ExchangeRates.Web          # https://localhost:7244 / http://localhost:5244
dotnet test                                      # all tests (xUnit)
dotnet test --filter "FullyQualifiedName~CurrencyTests.CreateCurrency_CorrectAdd_Ok"   # single test
```

No linter or formatter is configured. The app needs outbound internet access to reach NBP; if it fails, the page renders an empty view (errors are logged, not surfaced).

## Architecture

Layered solution; dependencies point inward: `Web` → `Services.Currency`, `Providers.Nbp`, `Data`, `Common`; `Services.Currency` → `Common`, `Data`; `Providers.Nbp` → `Services.Currency`.

- **ExchangeRates.Common** – abstractions only: `IMessageBroker`, `ICommand<T>`/`IQuery<T>`, `ICommandHandler`/`IQueryHandler`, `ICache`, `ServiceException`.
- **ExchangeRates.Data** – `ExchangeRatesContext` and entities (`CurrencyEntity`, `CurrencyRateEntity`). A rate row links a base currency (`Rates` on the owner) to a `FromCurrency`.
- **ExchangeRates.Services.Currency** – business logic as CQRS-style messages. Each file in `Commands/` and `Queries/` holds the message (a `struct`) **and** its handler class together. Also defines `ICurrencyProvider` (the provider abstraction), DTOs, domain exceptions, and `CurrencyOptions`.
- **ExchangeRates.Providers.Nbp** – `NbpProvider : ICurrencyProvider`. Only supports base currency `PLN` (throws `CurrencyCodeMismatch` otherwise).
- **ExchangeRates.Web** – composition root (`Program.cs`), the single MVC `CurrencyController` + view, the background service, and infrastructure adapters (`Infrastructure/Messaging/MediatR.cs` implements `IMessageBroker` over MediatR; `Infrastructure/Caching/MemoryCache.cs` implements `ICache` over `IMemoryCache`).

### Request / data flow

- Callers never use MediatR directly; they go through `IMessageBroker.SendCommandAsync/SendQueryAsync`. MediatR handler registration (pinned to 12.x, the last free-licensed line; v13+ needs a license key) scans both the Web assembly and the `Services.Currency` assembly (`Program.cs`), so a new handler in either is picked up automatically.
- **Write path**: `CurrencyRatesService` (hosted `BackgroundService`, polls every 1800s) ensures the default currency exists (`Currencies:DefaultCode` / `DefaultName` in `appsettings.json`, currently PLN), calls `ICurrencyProvider.GetRatesAsync`, then sends `CurrencyRatesUpdate`. Its handler upserts rates, then **invalidates the cache key `nameof(GetDefaultCurrency)`**.
- **Read path**: `CurrencyController.Index` → `GetDefaultCurrency` query. The handler caches the `CurrencyDetailDto` for 1 hour under that same key and applies sorting (`sortBy`: name/code/rate, `sortOrder`: asc/desc) *after* the cache lookup, mutating the cached DTO's `Rates`. Any new write that changes default-currency data must also remove that cache key.
- The background service uses its own DI scope per iteration (scoped services like the DbContext and provider) and swallows/logs all exceptions so the loop keeps running.

## Testing

Tests in `ExchangeRates.Tests` are integration-style: `Common/TestApplication` is a `WebApplicationFactory<Program>` that uses environment `Testing`, swaps in a separate InMemory DB (`"Tests"`), and **removes all `IHostedService`s** so the NBP poller does not run. Tests resolve `IMessageBroker` from a DI scope and send commands/queries end to end; the NBP provider is mocked with Moq where needed (never hit the network in tests). `Program` (top-level statements, internal) is exposed to the test project through `InternalsVisibleTo` in `ExchangeRates.Web.csproj`.
