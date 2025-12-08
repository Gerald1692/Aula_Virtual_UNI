using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AulaVirtualDAL;
using Microsoft.Extensions.Configuration;
using Entities;

namespace Aula_Virtual_UNI.Controllers
{
    public class MenuController : Controller
    {
        private readonly ProyectosDAL _proyectosDAL;
        private readonly TareasDAL _tareasDAL;
        private readonly IConfiguration _configuration;

        public MenuController(ProyectosDAL proyectosDAL, TareasDAL tareasDAL, IConfiguration configuration)
        {
            _proyectosDAL = proyectosDAL;
            _tareasDAL = tareasDAL;
            _configuration = configuration;
        }

        public IActionResult V_Menu()
        {
            // 1. Verificar si hay sesión activa
            var rol = HttpContext.Session.GetString("Rol");

            if (string.IsNullOrWhiteSpace(rol))
                return RedirectToAction("Login", "Login");

            var rolNormalizado = NormalizarRol(rol);

            // 2. Mostrar en consola el rol que llega
            Debug.WriteLine($"ROL EN SESIÓN -> '{rol}' | Normalizado -> '{rolNormalizado}'");

            // 3. Enviar datos a la vista
            ViewBag.Rol = rolNormalizado;
            ViewBag.Nombre = HttpContext.Session.GetString("Nombre");

            // 4. Si es estudiante, cargar sus proyectos y tareas asignadas
            if (rolNormalizado == "Estudiante")
            {
                var conexion = _configuration.GetConnectionString("ConexionDB");
                string idUsuarioStr = Request.Cookies["IdUsuario"];

                if (!string.IsNullOrEmpty(idUsuarioStr) && int.TryParse(idUsuarioStr, out int idUsuario))
                {
                    // Obtener proyectos asignados al estudiante
                    var proyectosRespuesta = _proyectosDAL.ObtenerProyectosPorEstudiante(idUsuario, conexion);
                    if (proyectosRespuesta != null && proyectosRespuesta.Ok)
                    {
                        ViewBag.ListaProyectos = proyectosRespuesta.ValorRetorno ?? new List<Entities.Proyectos>();
                    }
                    else
                    {
                        ViewBag.ListaProyectos = new List<Entities.Proyectos>();
                    }

                    // Obtener TODAS las tareas de los proyectos donde el estudiante está asignado
                    var tareasRespuesta = _tareasDAL.ObtenerTodasLasTareasDeProyectosDelEstudiante(idUsuario, conexion);
                    if (tareasRespuesta != null && tareasRespuesta.Ok)
                    {
                        ViewBag.ListaTareas = tareasRespuesta.ValorRetorno ?? new List<Entities.Tarea>();
                    }
                    else
                    {
                        ViewBag.ListaTareas = new List<Entities.Tarea>();
                    }
                }
                else
                {
                    ViewBag.ListaProyectos = new List<Entities.Proyectos>();
                    ViewBag.ListaTareas = new List<Entities.Tarea>();
                }
            }
            else
            {
                ViewBag.ListaProyectos = new List<Entities.Proyectos>();
                ViewBag.ListaTareas = new List<Entities.Tarea>();
            }

            return View(); // Retorna Views/Menu/V_Menu.cshtml
        }

        private static string NormalizarRol(string rol)
        {
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
    }
}
