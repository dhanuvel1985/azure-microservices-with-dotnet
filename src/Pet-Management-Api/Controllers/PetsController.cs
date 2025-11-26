using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Management_Api.DataAccess;
using Pet_Management_Api.DataAccess.Entities;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Pet_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController(ManagementDbContext mangementDb, ILogger<PetsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allPets = await mangementDb.Pets.Include(P => P.Breed).ToListAsync();
            return allPets != null ? Ok(allPets) : NotFound();
        }

        [HttpGet("{Id}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(int Id)
        {
            if (Id <= 0)
                return BadRequest();
            var pet = await mangementDb.Pets.Include(P => P.Breed).Where(_=>_.Id == Id).FirstOrDefaultAsync();
            return pet != null? Ok(pet) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewPet newPet)
        {
            try
            {
                var pet = newPet.ToPet();
                await mangementDb.AddAsync(pet);
                await mangementDb.SaveChangesAsync();
                return CreatedAtRoute(nameof(GetById), new { Id = pet.Id }, newPet);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public record NewPet(string Name, int Age, int BreedId)
        {
            public Pet ToPet()
            {
                return new Pet() { Name = Name, Age = Age, BreedId = BreedId};
            }
        }
    }
}
