using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aula_Virtual_UNI.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly ProyectosDal _proyectosDal;

        public ProyectosController(ProyectosDal proyectosDal)
        {
            _proyectosDal = proyectosDal;
        }

        public IActionResult V_Proyectos()
        {
            ObtenerProyectos();
            return View();
        }

        [HttpGet("api/proyectos")]
        public async Task<IActionResult> ObtenerProyectos()
        {
            var proyectos = await _proyectosDal.ObtenerProyectosAsync();
            return Ok(proyectos);
        }

        [HttpPost("api/proyectos")]
        public async Task<IActionResult> CrearProyecto([FromBody] Proyecto proyecto)
        {
            if (!ModelState.IsValid || proyecto == null)
            {
                return BadRequest("Datos de proyecto inválidos.");
            }

            var creado = await _proyectosDal.CrearProyectoAsync(proyecto);
            return Created($"/api/proyectos/{creado.Id}", creado);
        }

        [HttpPut("api/proyectos/{id:int}")]
        public async Task<IActionResult> ActualizarProyecto(int id, [FromBody] Proyecto proyecto)
        {
            if (!ModelState.IsValid || proyecto == null)
            {
                return BadRequest("Datos de proyecto inválidos.");
            }

            var actualizado = await _proyectosDal.ActualizarProyectoAsync(id, proyecto);
            if (actualizado == null)
            {
                return NotFound();
            }

            return Ok(actualizado);
        }

        [HttpDelete("api/proyectos/{id:int}")]
        public async Task<IActionResult> EliminarProyecto(int id)
        {
            var eliminado = await _proyectosDal.EliminarProyectoAsync(id);
            if (!eliminado)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
