using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region XPO Datenzugriff

var connectionString = builder.Configuration.GetConnectionString("WinLineConnectionString");

// Datalayer als Scoped
builder.Services.AddXpoDefaultDataLayer(ServiceLifetime.Scoped,
    options => DataLayerManager.CreateDataLayer(connectionString, true, false, false));

#endregion

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/companies", (ILogger<Program> logger, IConfiguration configuration, IDataLayer dataLayer) =>
{
    var companyNr = configuration.GetSection("WinLineSettings").GetValue<string>("Company");
    var mesoObjectLayer = new MesoObjectLayer(dataLayer, companyNr);
    using var uow = mesoObjectLayer.GetDataUnitOfWork();
    return Results.Ok(new XPQuery<ViewKontenstamm>(uow).Select(k => new { k.Kontonummer, k.Kontoname }).ToList());
})
.WithName("GetCompanies");

app.Run();
