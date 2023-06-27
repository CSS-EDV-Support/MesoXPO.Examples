using DevExpress.Xpo;
using MesoXPO;
using MesoXPO.Models;
using Microsoft.AspNetCore.Mvc;
using XpoTestWebApi.Controllers;

namespace XpoTestWebApi;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    
    private readonly ILogger<CompanyController> _logger;
    private readonly IDataLayer _dataLayer;
    private readonly string? _companyNr;
    private readonly MesoObjectLayer _mesoObjectLayer;

    public CompanyController(ILogger<CompanyController> logger, IConfiguration configuration, IDataLayer dataLayer)
    {
        _logger = logger;
        _dataLayer  = dataLayer;
        _companyNr = configuration.GetSection("WinLineSettings").GetValue<string>("Company");
        _mesoObjectLayer = new MesoObjectLayer(_dataLayer, _companyNr, 0);
    }

    [HttpGet(Name = "GetCompanies")]
    public IActionResult Get()
    {
        using var uow = _mesoObjectLayer.GetDataUnitOfWork();
        return Ok( new XPQuery<ViewKontenstamm>(uow).Select(k =>  new { Kontonummer = k.Kontonummer, Kontoname = k.Kontoname}).ToList());
    }
}