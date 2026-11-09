# 💱 ExchangeRates

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91)
![EF Core](https://img.shields.io/badge/EF%20Core-InMemory-68217A)
![Tests](https://img.shields.io/badge/tests-xUnit-green)

A small .NET 10 web app that displays current average exchange rates against the Polish zloty (PLN). Rates are fetched in the background from the public [NBP API](https://api.nbp.pl/) and shown on the main page in a sortable table.

The app uses an in-memory database, so there is nothing to install or configure. It is ready to run.

## ✨ Features

- **Automatic rate updates**: a background service downloads NBP table A every 30 minutes.
- **Sortable table**: click a column header (currency name, symbol, average rate) to sort ascending or descending.
- **Source and date**: the page shows the data provider and the effective date of the rates.
- **Caching**: the default currency view is cached for 1 hour and invalidated whenever new rates are saved.
- **Zero setup**: EF Core InMemory database, no migrations, no connection strings.

## 🚀 Getting started

**Requirements:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and internet access (to reach the NBP API).

```bash
git clone https://github.com/kapiziom/ExchangeRates.git
cd ExchangeRates
dotnet run --project ExchangeRates.Web
```

Then open <https://localhost:7244> (or <http://localhost:5244>). Rates appear right after the first background fetch completes; until then the page shows "Brak danych" ("No data").

## ⚙️ Configuration

Settings live in [ExchangeRates.Web/appsettings.json](ExchangeRates.Web/appsettings.json):

| Key | Default | Description |
| --- | --- | --- |
| `Currencies:DefaultCode` | `PLN` | Base currency code |
| `Currencies:DefaultName` | `Polski złoty` | Base currency display name |

> The NBP provider only supports `PLN` as the base currency.

## 🏗️ Architecture

The solution is split into layers; the web project is the composition root.

```
ExchangeRates.Web               MVC controller, views, background service, DI setup
ExchangeRates.Services.Currency Commands, queries and handlers (CQRS-style), DTOs, provider abstraction
ExchangeRates.Providers.Nbp     NBP API client implementing ICurrencyProvider
ExchangeRates.Data              EF Core context and entities
ExchangeRates.Common            Shared abstractions (message broker, caching, exceptions)
ExchangeRates.Tests             xUnit integration tests
```

How it works:

1. `CurrencyRatesService` (a hosted background service) asks the provider for the latest rates and sends a `CurrencyRatesUpdate` command.
2. The handler saves the rates and clears the cache.
3. The controller sends a `GetDefaultCurrency` query, which reads from the cache or the database, applies sorting, and renders the view.

## 🧪 Tests

```bash
dotnet test
```

Tests run against the full application using `WebApplicationFactory` with a separate in-memory database. The background service is disabled and the NBP provider is mocked, so no network access is needed.

## 🛠️ Tech stack

- .NET 10, ASP.NET Core MVC
- Entity Framework Core (InMemory provider)
- MediatR
- xUnit, Moq
- Bootstrap
