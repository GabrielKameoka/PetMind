using Microsoft.AspNetCore.Mvc;
using PetMind.API.Data;
using PetMind.API.Services;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CalculaPrecosService _calculaPrecosService;

    public ServicosController(AppDbContext context, CalculaPrecosService calculaPrecosService)
    {
        _context = context;
        _calculaPrecosService = calculaPrecosService;
    }
    
    [HttpGet("racas/{porte}")]
    public List<string> GetRacas(string porte) => _calculaPrecosService.GetRacasPorPorte(porte);

    [HttpGet("servicos/{porte}/{raca}")]
    public List<string> GetServicos(string porte, string raca) =>
        _calculaPrecosService.GetServicosPorRacaPorte(raca, porte);

    [HttpGet("adicionais")]
    public List<string> GetAdicionais() => _calculaPrecosService.GetServicosAdicionais();
}