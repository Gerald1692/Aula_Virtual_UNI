using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Runtime.CompilerServices;
using AulaVirtualDAL;

namespace Aula_Virtual_UNI.Controllers
{
    public class ProyectosController : Controller
    {

        private readonly ProyectosDAL AccesoDAL;
        private readonly UsuariosDAL UsuariosDAL;
        private readonly IConfiguration _configuration;

        public ProyectosController(ProyectosDAL proyectosDAL, UsuariosDAL usuariosDAL, IConfiguration configuration)
        {
            AccesoDAL = proyectosDAL;
            UsuariosDAL = usuariosDAL;
            _configuration = configuration;
        }



        public IActionResult V_Proyectos()
        {
            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = AccesoDAL.ObtenerProyectos(conexion);

                if (respuesta != null && respuesta.Ok && respuesta.ValorRetorno != null)
                {
                    ViewBag.ListaProyectos = respuesta.ValorRetorno;
                }
                else
                {
                    ViewBag.ListaProyectos = new List<Proyectos>();
                }

                // Obtener estudiantes para el dropdown
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
            catch
            {
                ViewBag.ListaProyectos = new List<Proyectos>();
                ViewBag.ListaEstudiantes = new List<Usuario>();
            }

            return View();
        }

        [HttpPost]
        public Respuesta<Proyectos> InsertarProyecto([FromBody] ProyectoDTO proyectoData)
        {

            Respuesta<Proyectos> reply = new Respuesta<Proyectos>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                string IdUsuario = Request.Cookies["IdUsuario"];
                
                // Crear objeto Proyectos y mapear los datos
                Proyectos Proyecto = new Proyectos();
                Proyecto.nombre = proyectoData.nombre;
                Proyecto.descripcion = proyectoData.descripcion;
                Proyecto.curso = proyectoData.curso;
                Proyecto.estado = proyectoData.estado ?? "pendiente";
                Proyecto.id_profesor = Convert.ToInt32(IdUsuario);
                Proyecto.id_asignado = string.Join(",", proyectoData.id_asignado);
                
                // Mapear la fecha: si viene como "fecha", usarla para fecha_finalizacion y establecer fecha_inicio como hoy
                if (!string.IsNullOrEmpty(proyectoData.fecha))
                {
                    if (DateTime.TryParse(proyectoData.fecha, out DateTime fechaFinalizacion))
                    {
                        Proyecto.fecha_finalizacion = fechaFinalizacion;
                        Proyecto.fecha_inicio = DateTime.Today;
                    }
                    else
                    {
                        reply.Ok = false;
                        reply.Mensaje = "La fecha proporcionada no es válida";
                        return reply;
                    }
                }
                else
                {
                    reply.Ok = false;
                    reply.Mensaje = "La fecha es requerida";
                    return reply;
                }

                var respuesta = AccesoDAL.InsertarProyecto(Proyecto, conexion);

                if (respuesta != null)
                {

                    reply = respuesta;


                }




            }
            catch (Exception ex)
            {

                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método InsertarProyecto {ex.Message}";
            }

            return reply;



        }

        [HttpPost]
        public Respuesta<List<Proyectos>> ObtenerProyectos()
        {

            Respuesta<List<Proyectos>> reply = new Respuesta<List<Proyectos>>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");


                var respuesta = AccesoDAL.ObtenerProyectos(conexion);

                if (respuesta != null)
                {

                    ViewBag.ListaProyectos = respuesta.ValorRetorno;


                }




            }
            catch (Exception ex)
            {

                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método ObtenerProyectos {ex.Message}";
            }

            return reply;



        }

        [HttpPost]
        public Respuesta<bool> EliminarProyecto([FromBody] int Id)
        {
            Respuesta<bool> reply = new Respuesta<bool>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = AccesoDAL.EliminarProyecto(Id, conexion);

                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método EliminarProyecto {ex.Message}";
            }

            return reply;
        }

        [HttpPost]
        public Respuesta<bool> AsignarEstudiante([FromBody] AsignacionEstudiante asignacion)
        {
            Respuesta<bool> reply = new Respuesta<bool>();
            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = AccesoDAL.AsignarEstudianteProyecto(asignacion.ProyectoId, asignacion.EstudianteId, conexion);
                if (respuesta != null)
                {
                    reply = respuesta;
                }
            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = $"Ha ocurrido un error en la capa del controlador en el método AsignarEstudiante: {ex.Message}";
            }
            return reply;
        }

    }

    // Clase auxiliar para recibir la asignación
    public class AsignacionEstudiante
    {
        public int ProyectoId { get; set; }
        public int EstudianteId { get; set; }
    }

    // Clase DTO para recibir los datos del formulario de proyecto
    public class ProyectoDTO
    {
        public string? nombre { get; set; }
        public string? curso { get; set; }
        public string? descripcion { get; set; }
        public string? fecha { get; set; }
        public string? estado { get; set; }
        public List<string>? id_asignado { get; set; }
    }
}
