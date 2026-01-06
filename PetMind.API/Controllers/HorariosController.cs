using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.Entities;
using PetMind.API.Services;
using PetMind.API.Services.Converters;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly CalculaPrecosService _calculaPrecosService;

    public HorariosController(AppDbContext context, IMapper mapper, CalculaPrecosService calculaPrecosService)
    {
        _context = context;
        _mapper = mapper;
        _calculaPrecosService = calculaPrecosService;
    }

    // GET: api/horarios
    [HttpGet]
    public async Task<ActionResult<List<HorarioResponseDto>>> GetAll(int page = 1, int pageSize = 10)
    {
        var horarios = await _context.Horarios
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(_mapper.Map<List<HorarioResponseDto>>(horarios));
    }

    // GET: api/horarios/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<HorarioResponseDto>> GetById(int id)
    {
        var horario = await _context.Horarios
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (horario == null) return NotFound();
        return Ok(_mapper.Map<HorarioResponseDto>(horario));
    }

    // POST: api/horarios
    [HttpPost]
    public async Task<ActionResult<HorarioResponseDto>> Create(CreateHorarioDto dto)
    {
        if (dto == null)
            return BadRequest("Dados do horário são obrigatórios");

        try
        {
            DateTime dataConvertida = DateTimeConverter.ConverterParaDateTime(dto.Data);

            if (dataConvertida < DateTime.Now)
                return BadRequest("Não é possível agendar horários no passado");

            var cachorro = await _context.Cachorros
                .FirstOrDefaultAsync(c => c.Id == dto.CachorroId);

            if (cachorro == null)
                return NotFound($"Cachorro com ID {dto.CachorroId} não encontrado");

            // Normaliza os dados do cachorro
            cachorro.Raca = cachorro.Raca?.Trim();
            cachorro.Porte = cachorro.Porte?.Trim();

            var petShopExiste = await _context.PetShops
                .AnyAsync(p => p.Id == dto.PetShopId);

            if (!petShopExiste)
                return NotFound($"PetShop com ID {dto.PetShopId} não encontrado");

            // Valida serviços disponíveis
            var servicosPermitidos = _calculaPrecosService.GetServicosPorRacaPorte(
                cachorro.Raca ?? "",
                cachorro.Porte ?? ""
            );

            // Se não encontrou serviços, verifica se a combinação existe
            if (!servicosPermitidos.Any())
            {
                var racasDisponiveis = _calculaPrecosService.GetRacasPorPorte(cachorro.Porte ?? "");
                var mensagem = $"Não foram encontrados serviços para '{cachorro.Raca}' de porte '{cachorro.Porte}'. ";

                if (racasDisponiveis.Any())
                    mensagem += $"Raças disponíveis para porte {cachorro.Porte}: {string.Join(", ", racasDisponiveis)}";
                else
                    mensagem += "Nenhuma raça cadastrada para este porte.";

                return BadRequest(mensagem);
            }

            // Verifica se o serviço solicitado está disponível
            if (!servicosPermitidos.Contains(dto.ServicoBaseSelecionado))
            {
                var servicosFormatados = string.Join(", ", servicosPermitidos);
                return BadRequest(
                    $"O serviço '{dto.ServicoBaseSelecionado}' não está disponível " +
                    $"para '{cachorro.Raca}' de porte '{cachorro.Porte}'. " +
                    $"Serviços disponíveis: {servicosFormatados}"
                );
            }

            // CRIA O HORÁRIO (apenas uma declaração)
            var horario = new Horario
            {
                CachorroId = dto.CachorroId,
                PetShopId = dto.PetShopId,
                Data = dataConvertida,
                ServicoBaseSelecionado = dto.ServicoBaseSelecionado,
                Adicionais = dto.Adicionais ?? new List<string>(),
                Cachorro = cachorro
            };

            horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);
            _context.Horarios.Add(horario);
            await _context.SaveChangesAsync();

            var horarioCompleto = await _context.Horarios
                .Include(h => h.Cachorro)
                .Include(h => h.PetShop)
                .FirstOrDefaultAsync(h => h.Id == horario.Id);

            return CreatedAtAction(nameof(GetById),
                new { id = horario.Id },
                _mapper.Map<HorarioResponseDto>(horarioCompleto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // PUT: api/horarios/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<HorarioResponseDto>> Update(int id, UpdateHorarioDto dto)
    {
        var horario = await _context.Horarios
            .Include(h => h.Cachorro)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (horario == null) return NotFound();

        // Se foi enviada uma nova data, converte
        if (!string.IsNullOrEmpty(dto.Data))
        {
            try
            {
                horario.Data = DateTimeConverter.ConverterParaDateTime(dto.Data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Atualiza outros campos
        if (!string.IsNullOrEmpty(dto.ServicoBaseSelecionado))
            horario.ServicoBaseSelecionado = dto.ServicoBaseSelecionado;

        if (dto.Adicionais != null)
            horario.Adicionais = dto.Adicionais;

        // Recalcula valor se necessário
        horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);

        await _context.SaveChangesAsync();

        var horarioAtualizado = await _context.Horarios
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync(h => h.Id == id);

        return Ok(_mapper.Map<HorarioResponseDto>(horarioAtualizado));
    }

    // DELETE: api/horarios/{id}
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