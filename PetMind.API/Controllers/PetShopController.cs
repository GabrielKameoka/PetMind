using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetMind.API.Data;
using PetMind.API.Models.Entities;

namespace PetMind.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetShopController : ControllerBase
{
    private readonly AppDbContext _context;

    public PetShopController(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    // GET: api/petshop
    [HttpGet]
    public async Task<ActionResult<List<PetShop>>> GetPetShops()
    {
        var petShops = await _context.PetShops
            .Include(p => p.Horarios)
            .ThenInclude(h => h.Cachorros)
            .ToListAsync();

        return Ok(petShops);
    }

    // GET: api/petshop/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PetShop>> GetPetShop(int id)
    {
        var petShop = await _context.PetShops
            .Include(p => p.Horarios)
            .ThenInclude(h => h.Cachorros)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (petShop == null)
        {
            return NotFound(new { message = "PetShop não encontrado" });
        }

        return Ok(petShop);
    }

    // POST: api/petshop
    [HttpPost]
    public async Task<ActionResult<PetShop>> CreatePetShop(PetShop petShop)
    {
        try
        {
            _context.PetShops.Add(petShop);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPetShop), new { id = petShop.Id }, petShop);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar PetShop", error = ex.Message });
        }
    }

    // PUT: api/petshop/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePetShop(int id, PetShop petShop)
    {
        try
        {
            var petShopExistente = await _context.PetShops.FindAsync(id);

            if (petShopExistente == null)
                return NotFound(new { message = "PetShop não encontrado" });

            petShopExistente.Email = petShop.Email;
            petShopExistente.Senha = petShop.Senha;
            petShopExistente.EnderecoPetShop = petShop.EnderecoPetShop;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!PetShopExists(id))
                return NotFound(new { message = "PetShop não encontrado durante a atualização" });

            return StatusCode(500, new { message = "Conflito de concorrência", error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar PetShop", error = ex.Message });
        }
    }

    // DELETE: api/petshop/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePetShop(int id)
    {
        var petShop = await _context.PetShops.FindAsync(id);
        if (petShop == null)
        {
            return NotFound();
        }

        _context.PetShops.Remove(petShop);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PetShopExists(int id)
    {
        return _context.PetShops.Any(e => e.Id == id);
    }
}