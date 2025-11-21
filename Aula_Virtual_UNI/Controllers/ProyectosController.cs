using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Runtime.CompilerServices;
using AulaVirtualDAL;

namespace Aula_Virtual_UNI.Controllers
{
    public class ProyectosController : Controller
    {

        private readonly ProyectosDAL AccesoDAL;
        private readonly IConfiguration _configuration;

        public ProyectosController(ProyectosDAL proyectosDAL, IConfiguration configuration)
        {
            AccesoDAL = proyectosDAL;
            _configuration = configuration;
        }



        public IActionResult V_Proyectos()
        {
            ObtenerProyectos();
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


    }
}
