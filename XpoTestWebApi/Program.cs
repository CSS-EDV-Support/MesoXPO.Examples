using MesoXPO;

uilder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region XPO Datenzugriff

var connectionString = builder.Configuration.GetConnectionString("WinLineConnectionString");

// Datalayer als Scoped
builder.Services.AddXpoDefaultDataLayer(ServiceLifetime.Scoped,
    options => DataLayerManager.CreateDataLayer(connectionString, true, false, false));

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
