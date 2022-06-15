using Comun;
using Microsoft.AspNetCore.Mvc;

namespace HttpClientDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public PersonasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Persona>>> Get()
        {
            return await Task.FromResult(new List<Persona>()
            {
                new Persona(){Id = 1, Nombre = "Juan"},
                new Persona(){Id = 2, Nombre = "José"},
                new Persona(){Id = 3, Nombre = "Jesús"},
            });
            //return await context.Personas.ToListAsync();
        }

        [HttpGet("[action]/{id:int}")]
        public async Task<ActionResult<Persona>> GetById(int id)
        {
            var p = context.Personas.Find(id);
            if (p == null)
                return BadRequest("Persona no existe");
            return await Task.FromResult(p);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> CreatePerson([FromBody] Persona persona)
        {
            if (context.Personas.Any(x => x.Id == persona.Id))
                return BadRequest("Persona ya existe");
            await context.Personas.AddAsync(persona);
            await context.SaveChangesAsync();
            return CreatedAtAction("GetById", new { id = persona.Id }, persona);
        }
    }
}
