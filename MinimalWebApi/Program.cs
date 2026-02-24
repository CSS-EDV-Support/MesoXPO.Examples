// MinimalWebApi - Einfachstes MesoXPO WebAPI Beispiel
// Zeigt die empfohlene Einrichtung mit AddMesoXPOServices() und ISystemDatabaseService.
// Fuer fortgeschrittene Multi-Mandant-Szenarien siehe das AdvancedWebApi-Projekt.

using DevExpress.Xpo;
using MesoXPO.Helper.Extensions;
using MesoXPO.Models;
using MesoXPO.Services;

var builder = WebApplication.CreateBuilder(args);

// Alle MesoXPO-Services per Dependency Injection registrieren.
// Verwendet die ConnectionStrings "WinLineXPOSystemConnection" und "WinLineXPODataConnection"
// aus der appsettings.json. Thread-sicherer DataLayer ist fuer Web APIs Pflicht.
builder.Services.AddMesoXPOServices(builder.Configuration);

// Swagger/OpenAPI fuer die API-Dokumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger-UI in der Entwicklungsumgebung aktivieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint: Artikel eines Mandanten abfragen
// ISystemDatabaseService wird per DI injiziert und bietet Zugriff auf die Systemdatenbank.
// Ueber GetMesoObjectLayer() wird ein mandantenspezifischer ObjectLayer erzeugt,
// der automatisch nach Mandant und Wirtschaftsjahr filtert.
// Eine manuelle Filterung auf Mesocomp/Mesoyear ist daher nicht noetig.
app.MapGet("/artikel/{mandant}", async (ISystemDatabaseService systemDb, string mandant) =>
{
    var mesoObjectLayer = systemDb.GetMesoObjectLayer(mandant);
    using var uow = mesoObjectLayer.GetDataUnitOfWork();

    var artikel = await new XPQuery<ArtikelStammdatei>(uow)
        .Select(a => new { a.Artikelnummer, a.Bezeichnung })
        .ToListAsync();

    return Results.Ok(artikel);
})
.WithName("GetArtikel");

// Endpoint: Kunden eines Mandanten abfragen
app.MapGet("/kunden/{mandant}", async (ISystemDatabaseService systemDb, string mandant) =>
{
    var mesoObjectLayer = systemDb.GetMesoObjectLayer(mandant);
    using var uow = mesoObjectLayer.GetDataUnitOfWork();

    var kunden = await new XPQuery<ViewKontenstamm>(uow)
        .Select(k => new { k.Kontonummer, k.Kontoname })
        .ToListAsync();

    return Results.Ok(kunden);
})
.WithName("GetKunden");

// Endpoint: Einzelnen Artikel per Artikelnummer abfragen
app.MapGet("/artikel/{mandant}/{artikelnummer}", async (ISystemDatabaseService systemDb, string mandant, string artikelnummer) =>
{
    var mesoObjectLayer = systemDb.GetMesoObjectLayer(mandant);
    using var uow = mesoObjectLayer.GetDataUnitOfWork();

    var artikel = await new XPQuery<ArtikelStammdatei>(uow)
        .Where(a => a.Artikelnummer == artikelnummer)
        .Select(a => new { a.Artikelnummer, a.Bezeichnung, Gruppe = a.Preisstamm.ArtikelgruppenStamm.Gruppentext })
        .FirstOrDefaultAsync();

    return artikel is not null ? Results.Ok(artikel) : Results.NotFound();
})
.WithName("GetArtikelByNummer");

// Endpoint: Einzelnen Kunden per Kontonummer abfragen
app.MapGet("/kunden/{mandant}/{kontonummer}", async (ISystemDatabaseService systemDb, string mandant, string kontonummer) =>
{
    var mesoObjectLayer = systemDb.GetMesoObjectLayer(mandant);
    using var uow = mesoObjectLayer.GetDataUnitOfWork();

    var kunde = await new XPQuery<ViewKontenstamm>(uow)
        .Where(k => k.Kontonummer == kontonummer)
        .Select(k => new { k.Kontonummer, k.Kontoname, k.Strasse, k.Ort, k.EMail })
        .FirstOrDefaultAsync();

    return kunde is not null ? Results.Ok(kunde) : Results.NotFound();
})
.WithName("GetKundeByNummer");

app.Run();
