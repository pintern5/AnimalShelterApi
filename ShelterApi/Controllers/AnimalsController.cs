using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;

namespace ShelterApi.Controllers
{
    [Route("animals")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public AnimalsController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpPost("newAnimal")]
        public async Task<IActionResult> NewAnimal(NewAnimalDto newAnimalDto)
        {
            try
            {
                var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == newAnimalDto.ShelterId);
                if (shelter == null)
                {
                    return StatusCode(404, new { message = "A menhely nem letezik" });
                }

                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = newAnimalDto.Name,
                    Species = newAnimalDto.Species,
                    Age = newAnimalDto.Age,
                    Weight = newAnimalDto.Weight,
                    ShelterId = newAnimalDto.ShelterId,
                    RegTime = DateTime.Now
                };

                await _context.Animals.AddAsync(animal);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "allat sikeresen rogzitve" });
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }

        [HttpGet("allAnimal")]
        public async Task<IActionResult> AllAnimal()
        {
            try
            {
                var animal = await _context.Animals.ToListAsync();

                if (animal != null && animal.Count > 0)
                {
                    return StatusCode(200, new { message = "Sikeres lekerdezes" });
                }

                return StatusCode(404, new { message = "Nincs rogzitett allat" });
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }

        [HttpGet("byId")]
        public async Task<IActionResult> ById([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);

                if (animal != null)
                {
                    return StatusCode(200, new { message = "Sikeres lekerdezes" });
                }

                return StatusCode(404, new { message = "Nincs ilyem allat" });
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] Guid id, UpdateAnimalDto updateAnimalDto)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                {
                    return StatusCode(404, new { message = "Nincs ilyen allat" });
                }

                var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == updateAnimalDto.ShelterId);
                if (shelter == null)
                {
                    return StatusCode(404, new { message = "A menhely nem letezik" });
                }

                animal.Name = updateAnimalDto.Name;
                animal.Species = updateAnimalDto.Species;
                animal.Age = updateAnimalDto.Age;
                animal.Weight = updateAnimalDto.Weight;
                animal.ShelterId = updateAnimalDto.ShelterId;

                _context.Animals.Update(animal);
                await _context.SaveChangesAsync();

                return StatusCode(200, new { message = "Allat sikeresen modositva" });
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }

        [HttpDelete("deleteAnimal")]
        public async Task<IActionResult> DeleteAnimal([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);

                if (animal != null)
                {
                    _context.Animals.Remove(animal);
                    await _context.SaveChangesAsync();

                    return StatusCode(200, new { message = "Allat sikeresen torolve" });
                }

                return StatusCode(404, new { message = "Nincs ilyen allat" });
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }

        [HttpGet("allAnimalsWithShelter")]
        public async Task<IActionResult> AllAnimalsWithShelter()
        {
            try
            {
                var result = await _context.Animals
                    .Join(_context.Shelters,
                        animal => animal.ShelterId,
                        shelter => shelter.Id,
                        (animal, shelter) => new
                        {
                            animal.Id,
                            animal.Name,
                            animal.Species,
                            animal.Age,
                            animal.Weight,
                            animal.RegTime,
                            ShelterName = shelter.Name
                        })
                    .ToListAsync();

                return StatusCode(200, new { message = "Sikeres lekerdezes"});
            }
            catch (Exception e)
            {
                return StatusCode(400, new { message = e.Message });
            }
        }
    }
}