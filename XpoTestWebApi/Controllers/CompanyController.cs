using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models;
using Microsoft.AspNetCore.Mvc;

namespace XpoTestWebApi;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    readonly string? _companyNr;
    readonly IDataLayer _dataLayer;

    readonly ILogger<CompanyController> _logger;
    readonly MesoObjectLayer _mesoObjectLayer;

    public CompanyController(ILogger<CompanyController> logger, IConfiguration configuration, IDataLayer dataLayer)
    {
        _logger = logger;
        _dataLayer = dataLayer;
        _companyNr = configuration.GetSection("WinLineSettings").GetValue<string>("Company");
        _mesoObjectLayer = new MesoObjectLayer(_dataLayer, _companyNr);
    }

    [HttpGet(Name = "GetCompanies")]
    public IActionResult Get()
    {
        using var uow = _mesoObjectLayer.GetDataUnitOfWork();
        return Ok(new XPQuery<ViewKontenstamm>(uow).Select(k => new { k.Kontonummer, k.Kontoname }).ToList());
    }
}
