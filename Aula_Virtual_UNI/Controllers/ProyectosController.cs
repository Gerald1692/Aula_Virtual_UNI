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
    }
}
