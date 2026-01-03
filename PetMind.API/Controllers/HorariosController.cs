using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.Entities;
using PetMind.API.Services;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper; 
    private readonly CalculaPrecosService _calculaPrecosService;

    public HorariosController(
        AppDbContext context, 
        IMapper mapper,
        CalculaPrecosService calculaPrecosService)
    {
        _context = context;
        _mapper = mapper;
        _calculaPrecosService = calculaPrecosService;
    }

    [HttpGet]
    public async Task<ActionResult<List<HorarioResponseDto>>> GetAll(int page = 1, int pageSize = 10)
    {
        var horarios = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return Ok(_mapper.Map<List<HorarioResponseDto>>(horarios));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HorarioResponseDto>> GetById(int id)
    {
        var horario = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (horario == null) return NotFound();
        return Ok(_mapper.Map<HorarioResponseDto>(horario));
    }

    [HttpPost]
    public async Task<ActionResult<HorarioResponseDto>> Post(CreateHorarioDto dto) 
    {
        // Validações iniciais
        if (dto == null)
            return BadRequest("Dados do horário são obrigatórios");

        if (dto.Data < DateTime.Now)
            return BadRequest("Não é possível agendar horários no passado");

        // Busca o cachorro específico
        var cachorro = await _context.Cachorros
            .FirstOrDefaultAsync(c => c.Id == dto.CachorroId);

        if (cachorro == null)
            return NotFound($"Cachorro com ID {dto.CachorroId} não encontrado");

        // Verifica se PetShop existe
        var petShopExiste = await _context.PetShops
            .AnyAsync(p => p.Id == dto.PetShopId);

        if (!petShopExiste)
            return NotFound($"PetShop com ID {dto.PetShopId} não encontrado");

        // Valida serviços disponíveis usando seu método real
        var servicosPermitidos = _calculaPrecosService.GetServicosPorRacaPorte(
            cachorro.Raca,
            cachorro.Porte
        );

        if (!servicosPermitidos.Contains(dto.ServicoBaseSelecionado))
        {
            return BadRequest(
                $"O serviço '{dto.ServicoBaseSelecionado}' não está disponível " +
                $"para {cachorro.Raca} de porte {cachorro.Porte}. " +
                $"Serviços disponíveis: {string.Join(", ", servicosPermitidos)}"
            );
        }

        // Validação básica de adicionais
        if (dto.Adicionais != null && dto.Adicionais.Any())
        {
            var adicionaisInvalidos = dto.Adicionais
                .Where(a => string.IsNullOrWhiteSpace(a))
                .ToList();

            if (adicionaisInvalidos.Any())
                return BadRequest("Alguns adicionais estão vazios ou inválidos");
        }

        // Cria o horário
        var horario = new Horario
        {
            CachorroId = dto.CachorroId,
            PetShopId = dto.PetShopId,
            Data = dto.Data,
            ServicoBaseSelecionado = dto.ServicoBaseSelecionado,
            Adicionais = dto.Adicionais ?? new List<string>(),
            Cachorros = new List<Cachorro> { cachorro }
        };

        horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);
        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();

        // Carrega relacionamentos para mapear para DTO
        var horarioCompleto = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == horario.Id);

        return CreatedAtAction(nameof(GetById), 
            new { id = horario.Id }, 
            _mapper.Map<HorarioResponseDto>(horarioCompleto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HorarioResponseDto>> Update(int id, UpdateHorarioDto dto)
    {
        var horario = await _context.Horarios
            .Include(h => h.Cachorros)
            .FirstOrDefaultAsync(h => h.Id == id);
            
        if (horario == null) return NotFound();
        
        // Mapeia DTO para entidade
        _mapper.Map(dto, horario);
        
        // Recalcula valor se necessário
        if (dto.ServicoBaseSelecionado != null || dto.Adicionais != null)
            horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);
        
        await _context.SaveChangesAsync();
        
        // Recarrega para retornar DTO completo
        var horarioAtualizado = await _context.Horarios
            .Include(h => h.Cachorros)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);
            
        return Ok(_mapper.Map<HorarioResponseDto>(horarioAtualizado));
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
}