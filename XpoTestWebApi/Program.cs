// XpoTestWebApi - Controller-basierte WebAPI mit MesoXPO
// Zeigt die Verwendung von AddMesoXPOServices() mit einem klassischen ApiController.
// Fuer das einfachste Minimal-API-Beispiel siehe das MinimalWebApi-Projekt.

using MesoXPO.Helper.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MesoXPO-Services per DI registrieren
builder.Services.AddMesoXPOServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger-UI in der Entwicklungsumgebung aktivieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
