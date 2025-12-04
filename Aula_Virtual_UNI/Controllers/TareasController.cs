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

            // Si es estudiante, solo mostrar las tareas que creó
            if (!string.IsNullOrEmpty(rol) && rol == "1" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
            {
                var tareasRespuesta = AccesoDAL.ObtenerTareasCreadasPorEstudiante(idUsuario, conexion);
                if (tareasRespuesta != null && tareasRespuesta.Ok)
                {
                    ViewBag.ListaTareas = tareasRespuesta.ValorRetorno ?? new List<Tarea>();
                }
                else
                {
                    ViewBag.ListaTareas = new List<Tarea>();
                }

                // Los estudiantes solo ven proyectos en los que están asignados
                var proyectosRespuesta = ProyectosDAL.ObtenerProyectosPorEstudiante(idUsuario, conexion);
                if (proyectosRespuesta != null && proyectosRespuesta.Ok)
                {
                    ViewBag.ListaProyectos = proyectosRespuesta.ValorRetorno ?? new List<Proyectos>();
                }
                else
                {
                    ViewBag.ListaProyectos = new List<Proyectos>();
                }
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
            }

            // Obtener estudiantes para el dropdown (solo para profesores)
            if (string.IsNullOrEmpty(rol) || rol != "1")
            {
                var estudiantes = UsuariosDAL.ObtenerEstudiantes(conexion);
                if (estudiantes != null && estudiantes.Ok)
                {
                    ViewBag.ListaEstudiantes = estudiantes.ValorRetorno;
                }
            }

            // Pasar el rol a la vista
            ViewBag.Rol = rol;

            return View();
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

                // Si es estudiante, usar su propio ID como el que crea la tarea
                if (!string.IsNullOrEmpty(rol) && rol == "1" && !string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
                {
                    Tarea.EstudianteAsignadoId = idUsuario;
                }

                var respuesta = AccesoDAL.InsertarTarea(Tarea, conexion);

                if (respuesta != null)
                {

                    reply = respuesta;


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
                var respuesta = AccesoDAL.ActualizarTarea(Tarea, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
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
    }
}
