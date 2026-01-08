using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public CachorrosController(AppDbContext context, IMapper mapper, IValidaRacaService validacaoService)
    {
        _context = context;
        _mapper = mapper;
        _validacaoService = validacaoService;
    }

    // GET: api/Cachorros
    [HttpGet]
    public async Task<ActionResult<List<CachorroResponseDto>>> GetAll()
    {
        var cachorros = await _context.Cachorros.ToListAsync();
        var response = _mapper.Map<List<CachorroResponseDto>>(cachorros);
        return Ok(response);
    }

    // GET: api/Cachorros/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CachorroResponseDto>> GetById(int id)
    {
        var cachorro = await _context.Cachorros.FindAsync(id);
        if (cachorro == null) return NotFound();

        return Ok(_mapper.Map<CachorroResponseDto>(cachorro));
    }
    
    // GET: api/Cachorros/{id}/PetShops
    [HttpGet("{id}/PetShops")]
    public async Task<ActionResult<List<CachorroResponseDto>>> GetByPetShopId(int id)
    {
        var petShopExiste = await _context.PetShops.AnyAsync(p => p.Id == id);
        if (!petShopExiste)
        {
            return NotFound(new { Message = $"PetShop com ID {id} não encontrado." });
        }

        var cachorros = await _context.Cachorros
            .Where(c => c.PetShopId == id)
            .ToListAsync();

        if (!cachorros.Any())
        {
            return Ok(new List<CachorroResponseDto>());
        }

        var response = _mapper.Map<List<CachorroResponseDto>>(cachorros);
        return Ok(response);
    }

    // POST: api/Cachorros
    [HttpPost]
    public async Task<ActionResult<CachorroResponseDto>> Create(CreateCachorroDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
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
        var cachorro = await _context.Cachorros.FindAsync(id);
        if (cachorro == null) return NotFound();

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
        var cachorro = await _context.Cachorros.FindAsync(id);
        if (cachorro == null) return NotFound();

        _context.Cachorros.Remove(cachorro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}