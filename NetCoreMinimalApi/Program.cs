using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using MesoXPO;
using MesoXPO.Helper;
using MesoXPO.Models;
using NetCoreMinimalApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region XPO Datenzugriff

var connectionString = builder.Configuration.GetConnectionString("WinLineConnectionString");
// Einmalige System-Registrierung
var systemConn = builder.Configuration.GetConnectionString("WinLineSystemConnectionString");

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

// Datalayer als Scoped
builder.Services.AddSingleton<ICompanyDataLayerProvider, CompanyDataLayerProvider>();

// Scoped UoW pro Request – holt sich das passende DAL für die Company aus AppSettings
builder.Services.AddScoped<DataUnitOfWork>(sp => {
    var cfg = sp.GetRequiredService<IConfiguration>();
    var companyNr = cfg.GetSection("WinLineSettings").GetValue<string>("Company");
    var dal = sp.GetRequiredService<ICompanyDataLayerProvider>().GetForCompany(companyNr!);
    return new DataUnitOfWork(dal);
});

// Scoped UoW pro Request – holt sich das passende DAL für die Company aus AppSettings
builder.Services.AddScoped<MesoObjectLayer>(sp => {
    var cfg = sp.GetRequiredService<IConfiguration>();
    var companyNr = cfg.GetSection("WinLineSettings").GetValue<string>("Company");
    return sp.GetRequiredService<ICompanyDataLayerProvider>().GetMesoObjectLayerForCompany(companyNr!);
});
builder.Services.AddXpoDefaultDataLayer(ServiceLifetime.Scoped,
    options => DataLayerManager.CreateDataLayer(connectionString, true, false, false));

#endregion

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

app.MapGet("/companies", async (ILogger<Program> logger, IDataLayer dataLayer) =>
    {
        using var uow = new UnitOfWork(dataLayer);
        return Results.Ok(await new XPQuery<Mandantenstamm>(uow).Select(k => new { k.Mandant, k.Nummer, k.Mesoprim }).ToListAsync());
    })
    .WithName("GetCompanies");

app.MapGet("/customer", async (ILogger<Program> logger, MesoObjectLayer context) =>
{
    using var uow = context.GetDataUnitOfWork();
    Console.WriteLine($"{uow.CompanyNr}, {uow.MesoYear}, {uow.BaseYear}");
    return Results.Ok(await new XPQuery<Kontenstamm>(uow).Where(k => k.Kennzeichen == "2" && k.Mesocomp == uow.CompanyNr && k.Mesoyear == uow.BaseYear).Select(k => new { k.Kontonummer, k.Kontoname, k.Mesoprim }).ToListAsync());
})
.WithName("GetCustomer");

app.Run();
