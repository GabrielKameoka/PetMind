using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.DTOs.PetShops;
using PetMind.API.Models.Entities;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetShopsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PetShopsController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/petshops
    [HttpGet]
    public async Task<ActionResult<List<PetShopBasicDto>>> GetAll()
    {
        var petShops = await _context.PetShops.ToListAsync();
        return Ok(_mapper.Map<List<PetShopBasicDto>>(petShops));
    }

    // GET: api/petshops/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PetShopResponseDto>> GetById(int id)
    {
        var petShop = await _context.PetShops
            .AsNoTracking() 
            .Include(p => p.Horarios)
            .ThenInclude(h => h.Cachorro)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (petShop == null) return NotFound();

        return Ok(_mapper.Map<PetShopResponseDto>(petShop));
    }

    // POST: api/petshops/
    [HttpPost]
    public async Task<ActionResult<PetShopResponseDto>> Create(CreatePetShopDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.PetShops.AnyAsync(p => p.Email == dto.Email))
            return BadRequest("Email já cadastrado");

        var petShop = _mapper.Map<PetShop>(dto);
        petShop.Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        _context.PetShops.Add(petShop);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),
            new { id = petShop.Id },
            _mapper.Map<PetShopResponseDto>(petShop));
    }

    // PUT: api/petshops/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<PetShopResponseDto>> Update(int id, UpdatePetShopDto dto)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.EnderecoPetShop))
            petShop.EnderecoPetShop = dto.EnderecoPetShop;

        // Validação para alteração de senha
        if (!string.IsNullOrEmpty(dto.NovaSenha))
        {
            if (string.IsNullOrEmpty(dto.ConfirmarNovaSenha))
                return BadRequest("Confirmação de senha é obrigatória");
        
            if (dto.NovaSenha != dto.ConfirmarNovaSenha)
                return BadRequest("As senhas não conferem");
        
            petShop.Senha = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
        }

        await _context.SaveChangesAsync();
        return Ok(_mapper.Map<PetShopResponseDto>(petShop));
    }

    // DELETE: api/petshops/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null) return NotFound();

        _context.PetShops.Remove(petShop);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    // GET: api/petshops/{id}/dashboard
    [HttpGet("{id}/dashboard")]
    public async Task<ActionResult<object>> GetDashboard(int id)
    {
        var petShop = await _context.PetShops
            .Include(p => p.Horarios)
            .ThenInclude(h => h.Cachorro)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (petShop == null) return NotFound();

        var resultado = new
        {
            PetShop = new
            {
                petShop.Id,
                petShop.Email,
                petShop.EnderecoPetShop
            },
            Estatisticas = new
            {
                TotalHorarios = petShop.Horarios.Count,
                HorariosHoje = petShop.Horarios
                    .Count(h => h.Data.Date == DateTime.Today),
                FaturamentoTotal = petShop.Horarios.Sum(h => h.ValorTotal),
                FaturamentoHoje = petShop.Horarios
                    .Where(h => h.Data.Date == DateTime.Today)
                    .Sum(h => h.ValorTotal)
            },
            ProximosHorarios = petShop.Horarios
                .Where(h => h.Data >= DateTime.Now)
                .OrderBy(h => h.Data)
                .Take(10)
                .Select(h => new
                {
                    h.Id,
                    h.Data,
                    h.ValorTotal,
                    h.ServicoBaseSelecionado,
                    CachorroNome = h.Cachorro != null ? h.Cachorro.NomeCachorro : "Desconhecido"
                })
                .ToList()
        };

        return Ok(resultado);
    }
}
