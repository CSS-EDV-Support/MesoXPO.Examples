using MesoXPO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#region XPO Datenzugriff

var connectionString = builder.Configuration.GetConnectionString("WinLineConnectionString");

// Datalayer als Scoped
builder.Services.AddXpoDefaultDataLayer(ServiceLifetime.Scoped,
    options => DataLayerManager.CreateDataLayer(connectionString, true, false, false));

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
