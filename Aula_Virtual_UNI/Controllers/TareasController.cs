using Microsoft.AspNetCore.Mvc;
using Entities;
using System.Runtime.CompilerServices;
using AulaVirtualDAL;

namespace Aula_Virtual_UNI.Controllers
{
    public class TareasController : Controller
    {

        private readonly TareasDAL AccesoDAL;
        private readonly IConfiguration _configuration;

        public TareasController(TareasDAL tareasDAL, IConfiguration configuration)
        {
            AccesoDAL = tareasDAL;
            _configuration = configuration;
        }



        public IActionResult V_Tareas()
        {
            ObtenerTareas();
            return View();
        }

        [HttpPost]
        public Respuesta<Tarea> InsertarTarea([FromBody] Tarea Tarea)
        {

            Respuesta<Tarea> reply = new Respuesta<Tarea>();

            try
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                string IdUsuario = Request.Cookies["IdUsuario"];
                Tarea.id_profesor = Convert.ToInt32(IdUsuario);

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


    }
}
