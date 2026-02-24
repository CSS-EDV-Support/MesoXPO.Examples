# MesoXPO.Examples

Beispielprojekte fuer das [MesoXPO-Framework](https://github.com/CSS-EDV-Support/MesoXPO) --
ein .NET ORM fuer den Zugriff auf Mesonic WinLine-Datenstrukturen.

(C) Tobias Forbrich - CSS EDV Support

## Voraussetzungen

- .NET 8.0 SDK oder hoeher
- DevExpress XPO (kostenlose ORM Library)
- SQL Server mit einer Mesonic WinLine-Datenbank
- MesoXPO NuGet Feed (Personal Access Token erforderlich)

Detaillierte Installationsanleitung: siehe [MesoXPO README](https://github.com/CSS-EDV-Support/MesoXPO#readme)

## Beispielprojekte

### MinimalWebApi (Einstieg empfohlen)

**Schwierigkeit:** Einfach | **Framework:** .NET 9, Minimal API

Das einfachste MesoXPO-Webprojekt. Zeigt die empfohlene Einrichtung mit
`AddMesoXPOServices()` und `ISystemDatabaseService`. Zwei Minimal-API-Endpoints
demonstrieren den Zugriff auf Artikel und Kunden eines Mandanten.

- **Highlights:** DI-Registrierung in einer Zeile, Swagger-UI, ISystemDatabaseService
- **Starten:** `dotnet run --project MinimalWebApi`
- **Swagger:** http://localhost:5180/swagger

### ConsoleXPO

**Schwierigkeit:** Einfach | **Framework:** .NET 8, Konsole

Interaktive Konsolenanwendung fuer den manuellen Verbindungsaufbau ohne
Dependency Injection. Ideal zum Experimentieren mit XPO-Abfragen.

- **Highlights:** SystemUnitOfWork, MesoObjectLayer, XPQuery-Abfragen
- **Starten:** `dotnet run --project ConsoleXPO`

### XpoTestWebApi

**Schwierigkeit:** Mittel | **Framework:** .NET 9, ASP.NET Core WebApi (Controller)

Controller-basierte WebAPI mit `AddMesoXPOServices()`. Zeigt den klassischen
Controller-Ansatz mit Constructor Injection von `ISystemDatabaseService`.

- **Highlights:** ApiController, ISystemDatabaseService, Swagger, Docker
- **Starten:** `dotnet run --project XpoTestWebApi`
- **Swagger:** http://localhost:5276/swagger

### AdvancedWebApi

**Schwierigkeit:** Fortgeschritten | **Framework:** .NET 9, Minimal API

Multi-Mandant-Szenario mit benutzerdefiniertem DataLayer-Caching.
Fuer Anwendungen, die gleichzeitig auf mehrere Mandanten-Datenbanken zugreifen muessen.

- **Highlights:** ConcurrentDictionary-Caching, CompanyDataLayerProvider, AddXpoCustomSession
- **Starten:** `dotnet run --project AdvancedWebApi`

### WinFormsXpoNet

**Schwierigkeit:** Mittel | **Framework:** .NET 8, Windows Forms

Desktop-Anwendung zum interaktiven Durchsuchen von MesoXPO-Datenobjekten
eines Mandanten. Zeigt den manuellen Verbindungsaufbau mit SystemUnitOfWork.

- **Highlights:** DataGridView-Binding, Reflection-basierte Objekterkennung, WinForms
- **Starten:** `dotnet run --project WinFormsXpoNet` (nur Windows)

## Konfiguration

### WebAPI-Projekte (MinimalWebApi, XpoTestWebApi)

Passen Sie die `appsettings.json` mit Ihren Datenbankverbindungen an:

```json
{
  "ConnectionStrings": {
    "WinLineXPOSystemConnection": "XpoProvider=MSSqlServer;Data Source=IHR-SERVER;Initial Catalog=CWLSYSTEM;User ID=meso;Password=IHR_PASSWORT;TrustServerCertificate=True",
    "WinLineXPODataConnection": "XpoProvider=MSSqlServer;Data Source=IHR-SERVER;Initial Catalog=CWLDATEN;User ID=meso;Password=IHR_PASSWORT;TrustServerCertificate=True"
  }
}
```

### AdvancedWebApi

Verwendet nur die System-ConnectionString (`WinLineSystemConnectionString`) und ermittelt
die Mandanten-Verbindung dynamisch ueber die Systemdatenbank.

### Konsolen- und Desktop-Projekte (ConsoleXPO, WinFormsXpoNet)

Die Verbindungsdaten werden zur Laufzeit interaktiv abgefragt.

## Weitere Informationen

- [MesoXPO Dokumentation](https://updates.itsolute.de/mesoxpo/doc/)
- [MesoXPO Business Dokumentation](https://updates.itsolute.de/mesoxpo-business/doc/)
- [DevExpress XPO Getting Started](https://docs.devexpress.com/XPO/2263/getting-started)
