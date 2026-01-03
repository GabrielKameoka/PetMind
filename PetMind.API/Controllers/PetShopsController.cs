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

    [HttpGet]
    public async Task<ActionResult<List<PetShopResponseDto>>> GetAll()
    {
        var petShops = await _context.PetShops.ToListAsync();
        return Ok(_mapper.Map<List<PetShopResponseDto>>(petShops));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PetShopResponseDto>> GetById(int id)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null) return NotFound();
        return Ok(_mapper.Map<PetShopResponseDto>(petShop));
    }

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

    [HttpPut("{id}")]
    public async Task<ActionResult<PetShopResponseDto>> Update(int id, UpdatePetShopDto dto)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null) return NotFound();

        // Email jamais poderá ser mutável
        if (!string.IsNullOrEmpty(dto.EnderecoPetShop))
            petShop.EnderecoPetShop = dto.EnderecoPetShop;
    
        if (!string.IsNullOrEmpty(dto.NovaSenha))
            petShop.Senha = BCrypt.Net.BCrypt.HashPassword(dto.NovaSenha);
    
        await _context.SaveChangesAsync();
        return Ok(_mapper.Map<PetShopResponseDto>(petShop));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null) return NotFound();

        _context.PetShops.Remove(petShop);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}