using Comun;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            //return await Task.FromResult(new List<Persona>()
            //{
            //    new Persona(){Id = 1, Nombre = "Juan"},
            //    new Persona(){Id = 2, Nombre = "José"},
            //    new Persona(){Id = 3, Nombre = "Jesús"},
            //});
            return await context.Personas.ToListAsync();
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<int>> Post(Persona persona)
        {
            context.Add(persona);
            await context.SaveChangesAsync();
            return persona.Id;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Persona persona)
        {
            Console.WriteLine("prueba");
            var personaExiste = await PersonaExiste(id);
            Console.WriteLine("personaExiste: " + personaExiste);

            if (!personaExiste)
            {
                return NotFound();
            }

            context.Update(persona);
            await context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> PersonaExiste(int id)
        {
            return await context.Personas.AnyAsync(p => p.Id == id);
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var personaExiste = await PersonaExiste(id);

            if (!personaExiste)
            {
                return NotFound();
            }

            context.Remove(new Persona() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
