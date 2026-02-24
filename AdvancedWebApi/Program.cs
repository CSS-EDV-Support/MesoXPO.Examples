// AdvancedWebApi - Fortgeschrittenes Multi-Mandant WebAPI Beispiel
// Zeigt ein benutzerdefiniertes Setup mit ConcurrentDictionary-Caching
// fuer performanten Zugriff auf verschiedene Mandanten-Datenbanken.
// Fuer das einfachste Beispiel siehe das MinimalWebApi-Projekt.

using AdvancedWebApi;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using MesoXPO;
using MesoXPO.Helper;
using MesoXPO.Models;
using MesoXPO.Models.SystemData;

var builder = WebApplication.CreateBuilder(args);

#region XPO Datenzugriff -- Erweiterte manuelle Konfiguration

var systemConn = builder.Configuration.GetConnectionString("WinLineSystemConnectionString");

// System-Datenbank: Enthaelt Mandanteninformationen, Benutzer etc.
var typesSystem = ConnectionHelper.GetPersistentTypesWinLineSystem();
var dictSystem = new ReflectionDictionary();
dictSystem.GetDataStoreSchema(typesSystem);

builder.Services.AddXpoCustomSession<SystemUnitOfWork>(
    true,
    options => options
        .UseConnectionString(systemConn)
        .UseThreadSafeDataLayer(true)
        .UseConnectionPool(true)
        .UseAutoCreationOption(AutoCreateOption.SchemaAlreadyExists)
        .UseEntityTypes(typesSystem)
);

// CompanyDataLayerProvider: Cached einen DataLayer pro Mandant
builder.Services.AddSingleton<ICompanyDataLayerProvider, CompanyDataLayerProvider>();

// Scoped MesoObjectLayer pro HTTP-Request
builder.Services.AddScoped<MesoObjectLayer>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var companyNr = cfg.GetSection("WinLineSettings").GetValue<string>("Company")
                   ?? throw new InvalidOperationException("WinLineSettings:Company nicht konfiguriert");
    return sp.GetRequiredService<ICompanyDataLayerProvider>().GetMesoObjectLayerForCompany(companyNr);
});

#endregion

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint: Alle Mandanten aus der Systemdatenbank auflisten
app.MapGet("/companies", async (SystemUnitOfWork systemUow) =>
{
    var mandanten = await new XPQuery<Datenbankverbindung>(systemUow)
        .Select(k => new { k.Mandant, k.Bezeichnung, k.VerbindungszeichenfolgeKurz })
        .ToListAsync();
    return Results.Ok(mandanten);
})
.WithName("GetCompanies")
;

// Endpoint: Kunden des konfigurierten Mandanten abfragen
app.MapGet("/customer", async (MesoObjectLayer mesoObjectLayer) =>
{
    using var uow = mesoObjectLayer.GetDataUnitOfWork();
    var kunden = await new XPQuery<Kontenstamm>(uow)
        .Where(k => k.Kennzeichen == "2" && k.Mesocomp == uow.CompanyNr && k.Mesoyear == uow.BaseYear)
        .Select(k => new { k.Kontonummer, k.Kontoname, k.Mesoprim })
        .ToListAsync();
    return Results.Ok(kunden);
})
.WithName("GetCustomer")
;

app.Run();
