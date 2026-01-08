using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.Cachorros;
using PetMind.API.Models.Entities;
using PetMind.API.Services;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CachorrosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidaRacaService _validacaoService;
    private readonly int _petShopIdLogado;

    public CachorrosController(AppDbContext context, IMapper mapper, IValidaRacaService validacaoService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _validacaoService = validacaoService;
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

    // GET: api/Cachorros
    [HttpGet]
    public async Task<ActionResult<List<CachorroResponseDto>>> GetAll()
    {
        var cachorros = await _context.Cachorros
            .Where(c => c.PetShopId == _petShopIdLogado) // 🔒 FILTRO
            .ToListAsync();

        return Ok(_mapper.Map<List<CachorroResponseDto>>(cachorros));
    }

    // GET: api/Cachorros/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CachorroResponseDto>> GetById(int id)
    {
        var cachorro = await _context.Cachorros
            .Where(c => c.PetShopId == _petShopIdLogado && c.Id == id)
            .FirstOrDefaultAsync();
        if (cachorro == null)
            return NotFound("Cachorro não encontrado ou você não tem permissão para visualizá-lo");

        return Ok(_mapper.Map<CachorroResponseDto>(cachorro));
    }

    // POST: api/Cachorros
    [HttpPost]
    public async Task<ActionResult<CachorroResponseDto>> Create(CreateCachorroDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        dto.PetShopId = _petShopIdLogado;

        // Validação customizada da raça
        if (!_validacaoService.RacaValida(dto.Raca))
        {
            var racasValidas = _validacaoService.GetRacasValidas();
            return BadRequest(new
            {
                Message = $"Raça '{dto.Raca}' não é válida.",
                RacasValidas = racasValidas
            });
        }

        // Validação do porte
        if (!_validacaoService.PorteValido(dto.Porte))
        {
            return BadRequest(new
            {
                Message = $"Porte '{dto.Porte}' não é válido.",
                PortesValidos = new[] { "Pequeno", "Médio", "Grande" }
            });
        }

        // Valida se raça é compatível com porte
        if (!_validacaoService.RacaCompativelComPorte(dto.Raca, dto.Porte))
        {
            var racasParaPorte = _validacaoService.GetRacasPorPorte(dto.Porte);
            return BadRequest(new
            {
                Message = $"Raça '{dto.Raca}' não está disponível para porte '{dto.Porte}'.",
                RacasDisponiveisParaPorte = racasParaPorte
            });
        }

        if (dto.PetShopId > 0)
        {
            var petShopExiste = await _context.PetShops.AnyAsync(p => p.Id == dto.PetShopId);
            if (!petShopExiste)
            {
                return BadRequest(new
                {
                    Message = $"PetShop com ID {dto.PetShopId} não encontrado.",
                    PetShopsDisponiveis = await _context.PetShops.Select(p => new { p.Id, p.Email }).ToListAsync()
                });
            }
        }

        var cachorro = _mapper.Map<Cachorro>(dto);

        _context.Cachorros.Add(cachorro);
        await _context.SaveChangesAsync();

        var responseDto = _mapper.Map<CachorroResponseDto>(cachorro);

        return CreatedAtAction(
            nameof(GetById),
            new { id = cachorro.Id },
            responseDto);
    }

    // PUT: api/Cachorros/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCachorroDto dto)
    {
        // Primeiro verifica se o cachorro existe e pertence ao petshop
        var cachorro = await _context.Cachorros
            .Where(c => c.PetShopId == _petShopIdLogado && c.Id == id)
            .FirstOrDefaultAsync();
        if (cachorro == null)
            return NotFound("Cachorro não encontrado ou você não tem permissão para editá-lo");

        // Valida raça se foi fornecida
        if (dto.Raca != null && !_validacaoService.RacaValida(dto.Raca))
        {
            var racasValidas = _validacaoService.GetRacasValidas();
            return BadRequest(new
            {
                Message = $"Raça '{dto.Raca}' não é válida.",
                RacasValidas = racasValidas
            });
        }

        // Valida porte se foi fornecido
        if (dto.Porte != null && !_validacaoService.PorteValido(dto.Porte))
        {
            return BadRequest(new
            {
                Message = $"Porte '{dto.Porte}' não é válido.",
                PortesValidos = new[] { "Pequeno", "Médio", "Grande" }
            });
        }

        // Valida compatibilidade se ambos foram fornecidos
        if (dto.Raca != null && dto.Porte != null &&
            !_validacaoService.RacaCompativelComPorte(dto.Raca, dto.Porte))
        {
            var racasParaPorte = _validacaoService.GetRacasPorPorte(dto.Porte);
            return BadRequest(new
            {
                Message = $"Raça '{dto.Raca}' não está disponível para porte '{dto.Porte}'.",
                RacasDisponiveisParaPorte = racasParaPorte
            });
        }

        _mapper.Map(dto, cachorro);
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<CachorroResponseDto>(cachorro));
    }

    // DELETE: api/Cachorros/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cachorro = await _context.Cachorros
            .Where(c => c.PetShopId == _petShopIdLogado && c.Id == id)
            .FirstOrDefaultAsync();
        if (cachorro == null)
            return NotFound("Cachorro não encontrado ou você não tem permissão para apagar-lo");

        // Verifica se o cachorro tem horários agendados futuros
        var temHorariosFuturos = await _context.Horarios
            .Where(h => h.CachorroId == id && h.PetShopId == _petShopIdLogado)
            .Where(h => h.Data >= DateTime.UtcNow)
            .AnyAsync();
        if (temHorariosFuturos)
        {
            return BadRequest(new
            {
                Message = "Não é possível excluir o cachorro pois ele tem horários agendados futuros.",
                Solucao = "Primeiro cancele ou transfira os horários agendados."
            });
        }

        _context.Cachorros.Remove(cachorro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}