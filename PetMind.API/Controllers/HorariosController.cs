using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Horarios;
using PetMind.API.Models.Entities;
using PetMind.API.Services;
using PetMind.API.Services.Converters;
using System.Security.Claims;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HorariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly CalculaPrecosService _calculaPrecosService;
    private readonly int _petShopIdLogado;

    public HorariosController(
        AppDbContext context,
        IMapper mapper,
        CalculaPrecosService calculaPrecosService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _calculaPrecosService = calculaPrecosService;
        _petShopIdLogado = ExtrairPetShopIdDoUsuario(httpContextAccessor?.HttpContext?.User);
    }

    private int ExtrairPetShopIdDoUsuario(ClaimsPrincipal user)
    {
        if (user == null)
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var petShopIdClaim = user.FindFirst("PetShopId")?.Value;
        if (string.IsNullOrEmpty(petShopIdClaim) || !int.TryParse(petShopIdClaim, out int petShopId))
            throw new UnauthorizedAccessException("PetShopId não encontrado no token");

        return petShopId;
    }

    // GET: api/horarios
    [HttpGet]
    public async Task<ActionResult<List<HorarioResponseDto>>> GetAll(int page = 1, int pageSize = 10)
    {
        var horarios = await _context.Horarios
            .Where(h => h.PetShopId == _petShopIdLogado)
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .OrderByDescending(h => h.Data)
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
            .Where(h => h.PetShopId == _petShopIdLogado && h.Id == id)
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync();

        if (horario == null)
            return NotFound("Horário não encontrado ou você não tem permissão para acessá-lo");

        return Ok(_mapper.Map<HorarioResponseDto>(horario));
    }

    // POST: api/horarios - SÓ PODE CRIAR NO MEU PETSHOP
    [HttpPost]
    public async Task<ActionResult<HorarioResponseDto>> Create(CreateHorarioDto dto)
{
    if (dto == null)
        return BadRequest("Dados do horário são obrigatórios");

    try
    {
        DateTime dataConvertida = DateTimeConverter.ConverterParaDateTime(dto.Data);

        if (dataConvertida < DateTime.UtcNow)
            return BadRequest("Não é possível agendar horários no passado");

        var cachorro = await _context.Cachorros
            .FirstOrDefaultAsync(c => c.Id == dto.CachorroId);

        if (cachorro == null)
            return NotFound($"Cachorro com ID {dto.CachorroId} não encontrado");

        // O cachorro pertence ao MEU petshop?
        if (cachorro.PetShopId != _petShopIdLogado)
            return BadRequest("Este cachorro não pertence ao seu petshop");

        // Normaliza os dados do cachorro
        cachorro.Raca = cachorro.Raca?.Trim();
        cachorro.Porte = cachorro.Porte?.Trim();

        // Valida serviços disponíveis
        var servicosPermitidos = _calculaPrecosService.GetServicosPorRacaPorte(
            cachorro.Raca ?? "",
            cachorro.Porte ?? ""
        );

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

        var horario = new Horario
        {
            CachorroId = dto.CachorroId,
            PetShopId = _petShopIdLogado, // Usa o ID do petshop logado e não o petshopid em si
            Data = dataConvertida,
            ServicoBaseSelecionado = dto.ServicoBaseSelecionado,
            Adicionais = dto.Adicionais ?? new List<string>(),
            Cachorro = cachorro
        };

        horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);
        _context.Horarios.Add(horario);
        await _context.SaveChangesAsync();

        var horarioCompleto = await _context.Horarios
            .Where(h => h.PetShopId == _petShopIdLogado && h.Id == horario.Id)
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync();

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
        // Verifica se existe e se á do petshop logado
        var horario = await _context.Horarios
            .Where(h => h.PetShopId == _petShopIdLogado && h.Id == id)
            .Include(h => h.Cachorro)
            .FirstOrDefaultAsync();

        if (horario == null)
            return NotFound("Horário não encontrado ou você não tem permissão para editá-lo");

        // Se foi enviada uma nova data, converte
        if (!string.IsNullOrEmpty(dto.Data))
        {
            try
            {
                var novaData = DateTimeConverter.ConverterParaDateTime(dto.Data);

                // Verifica se não é no passado
                if (novaData < DateTime.UtcNow)
                    return BadRequest("Não é possível agendar horários no passado");

                // Pode ter 50 cachorros no mesmo horário, se dane

                horario.Data = novaData;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Atualiza outros campos
        if (!string.IsNullOrEmpty(dto.ServicoBaseSelecionado))
        {
            // Valida se o serviço é válido para o cachorro
            var servicosPermitidos = _calculaPrecosService.GetServicosPorRacaPorte(
                horario.Cachorro?.Raca ?? "",
                horario.Cachorro?.Porte ?? ""
            );

            if (!servicosPermitidos.Contains(dto.ServicoBaseSelecionado))
            {
                var servicosFormatados = string.Join(", ", servicosPermitidos);
                return BadRequest(
                    $"O serviço '{dto.ServicoBaseSelecionado}' não está disponível " +
                    $"para este cachorro. Serviços disponíveis: {servicosFormatados}"
                );
            }

            horario.ServicoBaseSelecionado = dto.ServicoBaseSelecionado;
        }

        if (dto.Adicionais != null)
            horario.Adicionais = dto.Adicionais;

        // Recalcula valor se necessário
        horario.ValorTotal = _calculaPrecosService.GetPrecoTotalHorario(horario);

        await _context.SaveChangesAsync();

        var horarioAtualizado = await _context.Horarios
            .Where(h => h.PetShopId == _petShopIdLogado && h.Id == id)
            .Include(h => h.Cachorro)
            .Include(h => h.PetShop)
            .FirstOrDefaultAsync();

        return Ok(_mapper.Map<HorarioResponseDto>(horarioAtualizado));
    }

    // DELETE: api/horarios/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var horario = await _context.Horarios
            .Where(h => h.PetShopId == _petShopIdLogado && h.Id == id)
            .FirstOrDefaultAsync();

        if (horario == null)
            return NotFound("Horário não encontrado ou você não tem permissão para excluí-lo");

        _context.Horarios.Remove(horario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 🔧 ENDPOINT EXTRA: Buscar horários por data
    [HttpGet("por-data")]
    public async Task<ActionResult<List<HorarioResponseDto>>> GetByDate([FromQuery] string data)
    {
        try
        {
            DateTime dataFiltro = DateTimeConverter.ConverterParaDateTime(data);

            var horarios = await _context.Horarios
                .Where(h => h.PetShopId == _petShopIdLogado)
                .Where(h => EF.Functions.DateDiffDay(h.Data, dataFiltro) == 0)
                .Include(h => h.Cachorro)
                .Include(h => h.PetShop)
                .OrderBy(h => h.Data)
                .ToListAsync();

            return Ok(_mapper.Map<List<HorarioResponseDto>>(horarios));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 🔧 NOVO ENDPOINT: Ver quantos agendamentos tem em um horário (se quiser monitorar)
    [HttpGet("contagem-por-data-hora")]
    public async Task<ActionResult<object>> GetContagemPorDataHora([FromQuery] string data)
    {
        try
        {
            DateTime dataFiltro = DateTimeConverter.ConverterParaDateTime(data);

            var contagem = await _context.Horarios
                .Where(h => h.PetShopId == _petShopIdLogado)
                .Where(h => h.Data.Date == dataFiltro.Date)
                .GroupBy(h => new { h.Data.Year, h.Data.Month, h.Data.Day, h.Data.Hour })
                .Select(g => new
                {
                    DataHora = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                    Quantidade = g.Count(),
                    Cachorros = g.Select(h => h.Cachorro.NomeCachorro).ToList()
                })
                .OrderBy(x => x.DataHora)
                .ToListAsync();

            return Ok(contagem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}