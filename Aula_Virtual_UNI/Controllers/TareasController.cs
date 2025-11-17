using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aula_Virtual_UNI.Controllers
{
    public class TareasController : Controller
    {
        private readonly TareasDal _tareasDal;

        public TareasController(TareasDal tareasDal)
        {
            _tareasDal = tareasDal;
        }

        public IActionResult V_Tareas()
        {
            return View();
        }

        [HttpGet("api/tareas")]
        public async Task<IActionResult> ObtenerTareas()
        {
            var tareas = await _tareasDal.ObtenerTareasAsync();
            return Ok(tareas);
        }

        [HttpPost("api/tareas")]
        public async Task<IActionResult> CrearTarea([FromBody] Tarea tarea)
        {
            if (!ModelState.IsValid || tarea == null)
            {
                return BadRequest("Datos de tarea inválidos.");
            }

            tarea.Estado = string.IsNullOrWhiteSpace(tarea.Estado) ? "pendiente" : tarea.Estado;

            var creada = await _tareasDal.CrearTareaAsync(tarea);
            return Created($"/api/tareas/{creada.Id}", creada);
        }

        [HttpPut("api/tareas/{id:int}")]
        public async Task<IActionResult> ActualizarTarea(int id, [FromBody] Tarea tarea)
        {
            if (!ModelState.IsValid || tarea == null)
            {
                return BadRequest("Datos de tarea inválidos.");
            }

            tarea.Estado = string.IsNullOrWhiteSpace(tarea.Estado) ? "pendiente" : tarea.Estado;

            var actualizada = await _tareasDal.ActualizarTareaAsync(id, tarea);
            if (actualizada == null)
            {
                return NotFound();
            }

            return Ok(actualizada);
        }

        [HttpDelete("api/tareas/{id:int}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            var eliminada = await _tareasDal.EliminarTareaAsync(id);
            if (!eliminada)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
