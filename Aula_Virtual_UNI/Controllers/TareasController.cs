using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Runtime.CompilerServices;
using AulaVirtualDAL;

namespace Aula_Virtual_UNI.Controllers
{
    public class TareasController : Controller
    {

        private readonly TareasDAL AccesoDAL;
        private readonly ProyectosDAL ProyectosDAL;
        private readonly UsuariosDAL UsuariosDAL;
        private readonly IConfiguration _configuration;

        public TareasController(TareasDAL tareasDAL, ProyectosDAL proyectosDAL, UsuariosDAL usuariosDAL, IConfiguration configuration)
        {
            AccesoDAL = tareasDAL;
            ProyectosDAL = proyectosDAL;
            UsuariosDAL = usuariosDAL;
            _configuration = configuration;
        }



        public IActionResult V_Tareas()
        {
            var conexion = _configuration.GetConnectionString("ConexionDB");
            var rol = HttpContext.Session.GetString("Rol");
            string idUsuarioStr = Request.Cookies["IdUsuario"];

            // Normalizar el rol para comparación consistente
            var rolNormalizado = NormalizarRol(rol);

            // Si es estudiante, solo mostrar las tareas que creó
            if (rolNormalizado == "Estudiante" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
            {
                // Obtener solo las tareas creadas por este estudiante
                var tareasRespuesta = AccesoDAL.ObtenerTareasCreadasPorEstudiante(idUsuario, conexion);
                if (tareasRespuesta != null && tareasRespuesta.Ok)
                {
                    ViewBag.ListaTareas = tareasRespuesta.ValorRetorno ?? new List<Tarea>();
                }
                else
                {
                    ViewBag.ListaTareas = new List<Tarea>();
                }

                // Los estudiantes solo ven proyectos donde están asignados para crear tareas
                var proyectosRespuesta = ProyectosDAL.ObtenerProyectosAsignadosAlEstudiante(idUsuario, conexion);
                if (proyectosRespuesta != null && proyectosRespuesta.Ok)
                {
                    ViewBag.ListaProyectos = proyectosRespuesta.ValorRetorno ?? new List<Proyectos>();
                }
                else
                {
                    ViewBag.ListaProyectos = new List<Proyectos>();
                }

                // Para estudiantes, no cargar estudiantes inicialmente
                // Se cargarán dinámicamente cuando se seleccione un proyecto
                ViewBag.ListaEstudiantes = new List<Usuario>();
            }
            else
            {
                // Para profesores, obtener todas las tareas
                ObtenerTareas();

                // Obtener proyectos para el dropdown
                var proyectos = ProyectosDAL.ObtenerProyectos(conexion);
                if (proyectos != null && proyectos.Ok)
                {
                    ViewBag.ListaProyectos = proyectos.ValorRetorno;
                }

                // Para profesores, cargar todos los estudiantes inicialmente
                var estudiantes = UsuariosDAL.ObtenerEstudiantes(conexion);
                if (estudiantes != null && estudiantes.Ok)
                {
                    ViewBag.ListaEstudiantes = estudiantes.ValorRetorno;
                }
                else
                {
                    ViewBag.ListaEstudiantes = new List<Usuario>();
                }
            }

            // Pasar el rol a la vista
            ViewBag.Rol = rol;

            return View();
        }

        private static string NormalizarRol(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol))
                return string.Empty;

            var rolLimpio = rol.Trim();
            var rolLower = rolLimpio.ToLowerInvariant();

            return rolLower switch
            {
                "1" or "estudiante" or "alumno" => "Estudiante",
                "2" or "profesor" or "docente" => "Profesor",
                "3" or "admin" or "administrador" => "Administrador",
                _ => rolLimpio
            };
        }

        [HttpPost]
        public Respuesta<Tarea> InsertarTarea([FromBody] Tarea Tarea)
        {

            Respuesta<Tarea> reply = new Respuesta<Tarea>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var rol = HttpContext.Session.GetString("Rol");
                string idUsuarioStr = Request.Cookies["IdUsuario"];

                // Normalizar el rol para comparación consistente
                var rolNormalizado = NormalizarRol(rol);

                // Si es estudiante, establecer el creador y validar permisos
                if (rolNormalizado == "Estudiante" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
                {
                    // Establecer el creador de la tarea
                    Tarea.IdCreador = idUsuario;

                    // Validar que el estudiante esté asignado al proyecto
                    var proyectosRespuesta = ProyectosDAL.ObtenerProyectosAsignadosAlEstudiante(idUsuario, conexion);
                    if (proyectosRespuesta == null || !proyectosRespuesta.Ok || 
                        proyectosRespuesta.ValorRetorno == null || 
                        !proyectosRespuesta.ValorRetorno.Any(p => p.id_proyecto == Tarea.ProyectoId))
                    {
                        reply.Ok = false;
                        reply.Mensaje = "No tienes permiso para crear tareas en este proyecto. Solo puedes crear tareas en proyectos donde estás asignado.";
                        return reply;
                    }

                    // Validar que los estudiantes asignados pertenezcan al MISMO proyecto
                    List<int> estudiantesParaValidar = new List<int>();
                    if (Tarea.EstudiantesAsignadosIds != null && Tarea.EstudiantesAsignadosIds.Count > 0)
                    {
                        estudiantesParaValidar = Tarea.EstudiantesAsignadosIds;
                    }
                    else if (Tarea.EstudianteAsignadoId.HasValue && Tarea.EstudianteAsignadoId.Value > 0)
                    {
                        estudiantesParaValidar.Add(Tarea.EstudianteAsignadoId.Value);
                    }

                    foreach (var estudianteId in estudiantesParaValidar)
                    {
                        var estudianteAsignadoProyectos = ProyectosDAL.ObtenerProyectosAsignadosAlEstudiante(estudianteId, conexion);
                        if (estudianteAsignadoProyectos == null || !estudianteAsignadoProyectos.Ok || 
                            estudianteAsignadoProyectos.ValorRetorno == null ||
                            !estudianteAsignadoProyectos.ValorRetorno.Any(p => p.id_proyecto == Tarea.ProyectoId))
                        {
                            reply.Ok = false;
                            reply.Mensaje = "Solo puedes asignar tareas a estudiantes del mismo proyecto, no a integrantes de otros proyectos.";
                            return reply;
                        }
                    }
                }
                else if (rolNormalizado == "Profesor" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idProfesor))
                {
                    // Para profesores, también establecer el creador
                    Tarea.IdCreador = idProfesor;
                }

                var respuesta = AccesoDAL.InsertarTarea(Tarea, conexion);

                if (respuesta != null && respuesta.Ok && respuesta.ValorRetorno != null)
                {
                    reply = respuesta;

                    // Asignar estudiantes a la tarea
                    List<int> estudiantesParaAsignar = new List<int>();
                    
                    // Si hay múltiples estudiantes asignados, usar esa lista
                    if (Tarea.EstudiantesAsignadosIds != null && Tarea.EstudiantesAsignadosIds.Count > 0)
                    {
                        estudiantesParaAsignar = Tarea.EstudiantesAsignadosIds;
                    }
                    // Si solo hay un estudiante asignado, usar ese
                    else if (Tarea.EstudianteAsignadoId.HasValue && Tarea.EstudianteAsignadoId.Value > 0)
                    {
                        estudiantesParaAsignar.Add(Tarea.EstudianteAsignadoId.Value);
                    }

                    // Asignar los estudiantes a la tarea
                    if (estudiantesParaAsignar.Count > 0)
                    {
                        var asignacionRespuesta = AccesoDAL.AsignarEstudiantesATarea(respuesta.ValorRetorno.Id, estudiantesParaAsignar, conexion);
                        if (asignacionRespuesta != null && asignacionRespuesta.Ok)
                        {
                            // Actualizar el mensaje para incluir información sobre las asignaciones
                            reply.Mensaje += $" {asignacionRespuesta.Mensaje}";
                        }
                    }
                }




            }
            catch (Exception ex)
            {

                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método InsertarTarea {ex.Message}";
            }

            return reply;



        }

        [HttpPost]
        public Respuesta<List<Tarea>> ObtenerTareas()
        {

            Respuesta<List<Tarea>> reply = new Respuesta<List<Tarea>>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");


                var respuesta = AccesoDAL.ObtenerTareas(conexion);

                if (respuesta != null)
                {

                    ViewBag.ListaTareas = respuesta.ValorRetorno;


                }




            }
            catch (Exception ex)
            {

                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método ObtenerTareas {ex.Message}";
            }

            return reply;



        }



        [HttpPost]
        public Respuesta<bool> ActualizarTarea([FromBody] Tarea Tarea)
        {
            Respuesta<bool> reply = new Respuesta<bool>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var rol = HttpContext.Session.GetString("Rol");
                string idUsuarioStr = Request.Cookies["IdUsuario"];

                // Normalizar el rol para comparación consistente
                var rolNormalizado = NormalizarRol(rol);

                // Validar que solo el creador puede editar la tarea
                if (rolNormalizado == "Estudiante" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
                {
                    // Obtener la tarea para verificar el creador
                    var tareasRespuesta = AccesoDAL.ObtenerTareasCreadasPorEstudiante(idUsuario, conexion);
                    if (tareasRespuesta == null || !tareasRespuesta.Ok || 
                        tareasRespuesta.ValorRetorno == null ||
                        !tareasRespuesta.ValorRetorno.Any(t => t.Id == Tarea.Id && t.IdCreador == idUsuario))
                    {
                        reply.Ok = false;
                        reply.Mensaje = "No tienes permiso para editar esta tarea. Solo el creador puede editarla.";
                        return reply;
                    }
                }

                var respuesta = AccesoDAL.ActualizarTarea(Tarea, conexion);

                if (respuesta != null && respuesta.Ok)
                {
                    reply = respuesta;

                    // Actualizar asignaciones de estudiantes
                    // Primero eliminar todas las asignaciones existentes
                    var eliminarRespuesta = AccesoDAL.EliminarAsignacionesEstudiantes(Tarea.Id, conexion);
                    
                    // Luego asignar los nuevos estudiantes
                    List<int> estudiantesParaAsignar = new List<int>();
                    
                    // Si hay múltiples estudiantes asignados, usar esa lista
                    if (Tarea.EstudiantesAsignadosIds != null && Tarea.EstudiantesAsignadosIds.Count > 0)
                    {
                        estudiantesParaAsignar = Tarea.EstudiantesAsignadosIds;
                    }
                    // Si solo hay un estudiante asignado, usar ese
                    else if (Tarea.EstudianteAsignadoId.HasValue && Tarea.EstudianteAsignadoId.Value > 0)
                    {
                        estudiantesParaAsignar.Add(Tarea.EstudianteAsignadoId.Value);
                    }

                    // Asignar los estudiantes a la tarea
                    if (estudiantesParaAsignar.Count > 0)
                    {
                        var asignacionRespuesta = AccesoDAL.AsignarEstudiantesATarea(Tarea.Id, estudiantesParaAsignar, conexion);
                        if (asignacionRespuesta != null && asignacionRespuesta.Ok)
                        {
                            reply.Mensaje += $" {asignacionRespuesta.Mensaje}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método ActualizarTarea {ex.Message}";
            }

            return reply;
        }

        [HttpPost]
        public Respuesta<bool> EliminarTarea([FromBody] int Id)
        {
            Respuesta<bool> reply = new Respuesta<bool>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = AccesoDAL.EliminarTarea(Id, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método EliminarTarea {ex.Message}";
            }

            return reply;
        }

        [HttpPost]
        public Respuesta<bool> ActualizarEstadoTarea([FromBody] Tarea Tarea)
        {
            Respuesta<bool> reply = new Respuesta<bool>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                // Usamos Tarea.Id y Tarea.Estado del objeto recibido
                var respuesta = AccesoDAL.ActualizarEstadoTarea(Tarea.Id, Tarea.Estado, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método ActualizarEstadoTarea {ex.Message}";
            }

            return reply;
        }

        [HttpGet]
        public Respuesta<List<Usuario>> ObtenerEstudiantesTarea(int tareaId)
        {
            Respuesta<List<Usuario>> reply = new Respuesta<List<Usuario>>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = AccesoDAL.ObtenerEstudiantesPorTarea(tareaId, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Error al obtener estudiantes: {ex.Message}";
            }

            return reply;
        }

        [HttpGet]
        public Respuesta<List<Usuario>> ObtenerEstudiantesPorProyecto(int proyectoId)
        {
            Respuesta<List<Usuario>> reply = new Respuesta<List<Usuario>>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = ProyectosDAL.ObtenerEstudiantesDelProyectoCompleto(proyectoId, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Error al obtener estudiantes del proyecto: {ex.Message}";
            }

            return reply;
        }
    }
}
