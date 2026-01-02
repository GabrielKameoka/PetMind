using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.Entities;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CachorroController : ControllerBase
{
    private readonly AppDbContext _context;

    public CachorroController(AppDbContext context)
    {
        _context = context;
    }

    //GET: api/cachorro
    [HttpGet]
    public async Task<List<Cachorro>> GetAll() => await _context.Cachorros
        .ToListAsync();

    //GET: api/cachorro/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<List<Cachorro>>> GetById(int id)
    {
        var cachorro = await _context.Cachorros
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cachorro == null)
            return NotFound();

        return Ok(cachorro);
    }

    //POST: api/cachorro
    [HttpPost]
    public async Task<ActionResult<Cachorro>> CreateCachorro([FromBody] Cachorro cachorro)
    {
        try
        {
            _context.Add(cachorro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cachorro.Id }, cachorro);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar Cachorro", error = ex.Message });
        }
    }

    //PUT: api/cachorro/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCachorro(int id, Cachorro cachorro)
    {
        try
        {
            var cachorroExistente = await _context.Cachorros.FindAsync(id);

            if (cachorroExistente == null)
                return NotFound();

            cachorroExistente.NomeCachorro = cachorro.NomeCachorro;
            cachorroExistente.NomeTutor = cachorro.NomeTutor;
            cachorroExistente.ContatoTutor = cachorro.ContatoTutor;
            cachorroExistente.EnderecoCachorro = cachorro.EnderecoCachorro;
            cachorroExistente.Raca = cachorro.Raca;
            cachorroExistente.Porte = cachorro.Porte;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar Cachorro", error = ex.Message });
        }
    }
    
    //DELETE: api/cachorro/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCachorro(int id)
    {
        var cachorro = await _context.Cachorros.FindAsync(id);
        if (cachorro == null)
        {
            return NotFound();
        }

        _context.Cachorros.Remove(cachorro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    // sla se isso é rest
}