using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;

namespace ShelterApi.Controllers;

[Route("queries")]
[ApiController]
public class QueriesController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public QueriesController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpGet("allSheltersWithAnimals")]
    public async Task<IActionResult> AllSheltersWithAnimals()
    {
        try
        {
            var result = await _context.Shelters
                .Include(s => s.Animals)
                .ToListAsync();

            return Ok(new { message = "Sikeres lekerdezes"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Hiba {ex.Message}" });
        }
    }

    [HttpGet("allAnimalsWithShelter")]
    public async Task<IActionResult> AllAnimalsWithShelter()
    {
        try
        {
            var result = await _context.Animals
                .Join(_context.Shelters,
                    a => a.ShelterId,
                    s => s.Id,
                    (a, s) => new
                    {
                        a.Id,
                        a.Name,
                        a.Species,
                        a.Age,
                        a.Weight,
                        a.RegTime,
                        ShelterName = s.Name
                    })
                .ToListAsync();

            return Ok(new { message = "Sikeres lekerdezes"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Hiba {ex.Message}" });
        }
    }

    [HttpGet("animalsOfShelter")]
    public async Task<IActionResult> AnimalsOfShelter([FromQuery] Guid shelterId)
    {
        try
        {
            var shelter = await _context.Shelters
                .Include(s => s.Animals)
                .Where(s => s.Id == shelterId)
                .FirstOrDefaultAsync();

            if (shelter == null)
            {
                return NotFound(new { message = "Ezzel az idval nincs menhely" });
            }

            return Ok(new { message = "Sikeres lekerdezes"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Hiba {ex.Message}" });
        }
    }

    [HttpGet("animalCountByShelter")]
    public async Task<IActionResult> AnimalCountByShelter()
    {
        try
        {
            var result = await _context.Shelters
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    AnimalCount = s.Animals.Count()
                })
                .OrderByDescending(x => x.AnimalCount)
                .ToListAsync();

            return Ok(new { message = "Sikeres lekerdezes"});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Hiba {ex.Message}" });
        }
    }
}
