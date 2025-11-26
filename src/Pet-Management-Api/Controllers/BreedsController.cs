using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet_Management_Api.DataAccess;
using Pet_Management_Api.DataAccess.Entities;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Breed_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreedsController(ManagementDbContext mangementDb, ILogger<BreedsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allPets = await mangementDb.Breeds.ToListAsync();
            return allPets != null ? Ok(allPets) : NotFound();
        }

        [HttpGet("{Id}", Name = nameof(GetBreedById))]
        public async Task<IActionResult> GetBreedById(int Id)
        {
            if (Id <= 0)
                return BadRequest();
            var pet = await mangementDb.Breeds.FindAsync(Id);
            return pet != null? Ok(pet) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewBreed newBreed)
        {
            try
            {
                var breed = newBreed.ToBreed();
                await mangementDb.AddAsync(breed);
                await mangementDb.SaveChangesAsync();
                return CreatedAtRoute(nameof(GetBreedById), new { Id = breed.Id }, newBreed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public record NewBreed(string Name)
        {
            public Breed ToBreed()
            {
                return new Breed(0, Name );
            }
        }
    }
}
