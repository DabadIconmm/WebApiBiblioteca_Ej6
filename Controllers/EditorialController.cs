using Ejercicio_Sesión_1.DTOs;
using Ejercicio_Sesión_1.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ejercicio_Sesión_1.Controllers
{
    [ApiController]
    [Route("api/EditorialController")]
    public class EditorialController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly ILogger<EditorialController> logger;

        //4.2
        public EditorialController(ApplicationDbContext context, ILogger<EditorialController> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        //4.3.a
        [HttpGet("ListEditoriales")]
        public async Task<IEnumerable<Editorial>> Get()
        {
            logger.LogInformation("Obteniendo lista Editoriales");
            return await context.Editoriales.ToListAsync();
        }
        //4.3.b
        [HttpGet("{id:int}")] //asi restringimo el tipo de argumento que queremos recibir
        public async Task<ActionResult<Editorial>> GetEditorial(int id)
        {
            //var editorial = await context.Editoriales.SingleOrDefaultAsync();//esta es una de las opciones.
            ////esta es la segunda opcion
            //var editorial = await context.Editoriales.FirstOrDefaultAsync(x=>x.Id==id);
            //Para devolver un indice
            var editorial = await context.Editoriales.FindAsync(id);
            logger.LogInformation("Obteniendo Editoriales por ID: " + id);
            return Ok(editorial);
        }
        //4.3.c
        [HttpGet("contiene")]
        public async Task<ActionResult<IEnumerable<Editorial>>> GetEditorialContiene([FromQuery]string contiene)
        {
            var editorial = await context.Editoriales.Where(x => x.Nombre.Contains(contiene)).OrderBy(x => x.Nombre).ToListAsync();
            return Ok(editorial);
        }
        //4.3.d
        [HttpGet("getEditoriales/{id:int}")]
        public async Task<ActionResult<Editorial>> GetEditoriales(int id)
        {
            var editorial = await context.Editoriales.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
            if (editorial == null)
            {
                return NotFound();
            }
            return Ok(editorial);
        }
        //4.4.a DTOEditorialLibro
        [HttpGet("editorialeslibrosdto/{id:int}")]
        public async Task<ActionResult<Editorial>> GetEditorialesLibrosDTO(int id)
        {
            var editorial = await (from x in context.Editoriales
                                   select new DTOEditorialLibro
                                   {
                                       IdEditorial = x.Id,
                                       Nombre = x.Nombre,
                                       Libros = x.Libros.Select(y => new DTOLibroItem
                                       {
                                           IdLibro = y.Id,
                                           Nombre = y.Titulo,
                                           Paginas = y.Paginas
                                       }).ToList(),
                                   }).FirstOrDefaultAsync(x => x.IdEditorial == id);

            if (editorial == null)
            {
                return NotFound();
            }
            return Ok(editorial);
        }
        //4.5
        [HttpPost]
        public async Task<ActionResult> PostEditorial(Editorial editorial)
        {
            //estado de la familia.
            var statusEditorial = context.Entry(editorial).State; // Detached, sin seguimiento (viene de fuera y no se conoce)
            //Otra forma de hacer lo anterior
            //context.Entry(editorial).State = EntityState.Added;
            await context.AddAsync(editorial);
            var statusEditoria2 = context.Entry(editorial).State; // Added, agregado y conocido
            await context.SaveChangesAsync();
            var statusEditorial3 = context.Entry(editorial).State; // Unchanged. Cambiado y sin modificar
            return Ok();
        }

        //4.6
        [HttpPost("editoriallibros")]
        public async Task<ActionResult> PostDTOEditorialLibros(DTOEditorialLibro editorialLibros)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var newEditorial = new Editorial()
                {
                    Nombre = editorialLibros.Nombre
                };

                await context.AddAsync(newEditorial);
                await context.SaveChangesAsync();

                foreach (var libro in editorialLibros.Libros)
                {
                    var newLibro = new Libro()
                    {
                        Titulo = libro.Nombre,
                        Paginas = libro.Paginas,
                        Editorial = newEditorial
                    };

                    await context.AddAsync(newLibro);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Created("Editorial", new { editorial = newEditorial });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest("Se ha producido un error" + ex.Message);
            }
        }

        // 4.7
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutEditorial([FromRoute] int id, [FromBody] Editorial editorial)
        {
            if (id != editorial.Id)
            {
                return BadRequest("Los ids proporcionados son diferentes");
            }
            var existe = await context.Editoriales.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            context.Update(editorial);

            await context.SaveChangesAsync();
            return NoContent();
        }

        // 4.8
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var editorial = await context.Editoriales.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);

            if (editorial is null)
            {
                return NotFound();
            }

            if (editorial.Libros.Count > 0)
            {
                return BadRequest("Esta editorial contiene libros. No se puede eliminar");
            }

            context.Remove(editorial);
            await context.SaveChangesAsync();
            return Ok();
        }

        // 4.9.c
        [HttpDelete("Logico/{id:int}")]
        public async Task<ActionResult> DeleteLogico(int id)
        {
            var editorial = await context.Editoriales.AsTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (editorial is null)
            {
                return NotFound();
            }

            editorial.Eliminado = true;
            await context.SaveChangesAsync();
            return Ok();
        }

        // 4.9.e
        [HttpPut("Restaurar/{id:int}")]
        public async Task<ActionResult> Restaurar(int id)
        {
            var editorial = await context.Editoriales.AsTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (editorial is null)
            {
                return NotFound();
            }

            editorial.Eliminado = false;
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
