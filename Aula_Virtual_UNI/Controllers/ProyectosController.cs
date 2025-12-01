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
        public Respuesta<Proyectos> InsertarProyecto([FromBody] Proyectos Proyecto)
        {

            Respuesta<Proyectos> reply = new Respuesta<Proyectos>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                string IdUsuario = Request.Cookies["IdUsuario"];
                Proyecto.id_profesor = Convert.ToInt32(IdUsuario);

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
}
