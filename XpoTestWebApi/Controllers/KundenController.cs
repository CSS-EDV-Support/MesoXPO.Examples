// KundenController - Beispiel fuer einen Controller mit MesoXPO DI-Integration
// Zeigt den empfohlenen Weg: ISystemDatabaseService per Constructor Injection

using DevExpress.Xpo;
using MesoXPO.Models;
using MesoXPO.Services;
using Microsoft.AspNetCore.Mvc;

namespace XpoTestWebApi.Controllers;

/// <summary>
/// Beispiel-Controller: Liest Kunden (Kontenstamm) aus einem WinLine-Mandanten.
/// Verwendet ISystemDatabaseService fuer den DI-basierten Datenbankzugriff.
/// </summary>
[ApiController]
[Route("[controller]")]
public class KundenController : ControllerBase
{
    readonly ISystemDatabaseService _systemDb;
    readonly ILogger<KundenController> _logger;

    public KundenController(ISystemDatabaseService systemDb, ILogger<KundenController> logger)
    {
        _systemDb = systemDb;
        _logger = logger;
    }

    /// <summary>
    /// Gibt alle Kunden des angegebenen Mandanten zurueck.
    /// Der Mandant wird als Route-Parameter uebergeben.
    /// </summary>
    [HttpGet("{mandant}")]
    public IActionResult GetKunden(string mandant)
    {
        _logger.LogInformation("Lade Kunden fuer Mandant {Mandant}", mandant);

        var mesoObjectLayer = _systemDb.GetMesoObjectLayer(mandant);
        using var uow = mesoObjectLayer.GetDataUnitOfWork();

        var kunden = new XPQuery<ViewKontenstamm>(uow)
            .Select(k => new { k.Kontonummer, k.Kontoname })
            .ToList();

        return Ok(kunden);
    }
}
