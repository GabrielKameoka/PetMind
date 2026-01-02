using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.Entities;
using PetMind.API.Services;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorarioController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CalculaPrecosService _calculaPrecosService;

    public HorarioController(AppDbContext context, CalculaPrecosService calculaPrecosService)
    {
        _context = context;
        _calculaPrecosService = calculaPrecosService;
    }

    [HttpGet]
    public async Task<List<Horario>> GetAll(int page = 1, int pageSize = 10) => await _context.Horarios
        .Include(h => h.Cachorros)
        .Include(h => h.PetShop)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Horario>> GetById(int id)
    {
        var horario = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (horario == null) return NotFound();

        return Ok(horario);
    }

    [HttpPost]
    public async Task<ActionResult<Horario>> Post(CreateHorarioDto dto)
    {
        // Busca cachorros
        var cachorros = await _context.Cachorros
            .Where(c => dto.CachorroIds.Contains(c.Id))
            .ToListAsync();

        if (cachorros.Count != dto.CachorroIds.Count)
            return BadRequest("Cachorros não encontrados");

        // Verifica se PetShop existe
        var petShopExiste = await _context.PetShops.AnyAsync(p => p.Id == dto.PetShopId);
        if (!petShopExiste) return BadRequest("PetShop não encontrado");

        // 🔎 Validação do serviço base
        foreach (var cachorro in cachorros)
        {
            var servicosPermitidos = _calculaPrecosService.GetServicosPorRacaPorte(cachorro.Raca, cachorro.Porte);

            if (!servicosPermitidos.Contains(dto.ServicoBaseSelecionado))
            {
                return BadRequest(
                    $"O serviço '{dto.ServicoBaseSelecionado}' não está disponível para a raça {cachorro.Raca} ({cachorro.Porte}).");
            }
        }

        // Cria o horário
        var horario = new Horario
        {
            CachorroIds = dto.CachorroIds,
            PetShopId = dto.PetShopId,
            Data = dto.Data,
            ServicoBaseSelecionado = dto.ServicoBaseSelecionado,
            Adicionais = dto.Adicionais,
            Cachorros = cachorros
        };

        horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);

        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = horario.Id }, horario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Horario horario)
    {
        if (id != horario.Id) return BadRequest();

        var horarioExistente = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (horarioExistente == null) return NotFound();

        // Atualiza só o necessário
        horarioExistente.Data = horario.Data;
        horarioExistente.ServicoBaseSelecionado = horario.ServicoBaseSelecionado;
        horarioExistente.Adicionais = horario.Adicionais;
        horarioExistente.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var horario = await _context.Horarios.FindAsync(id);
        if (horario == null) return NotFound();

        _context.Horarios.Remove(horario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("racas/{porte}")]
    public List<string> GetRacas(string porte) => _calculaPrecosService.GetRacasPorPorte(porte);

    [HttpGet("servicos/{porte}/{raca}")]
    public List<string> GetServicos(string porte, string raca) =>
        _calculaPrecosService.GetServicosPorRacaPorte(raca, porte);

    [HttpGet("adicionais")]
    public List<string> GetAdicionais() => _calculaPrecosService.GetServicosAdicionais();
}