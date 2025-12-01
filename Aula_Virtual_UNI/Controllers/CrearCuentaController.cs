using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace Aula_Virtual_UNI.Controllers
{
    public class CrearCuentaController : Controller
    {
        private readonly RegistroDAL _registroDAL;
        private readonly IConfiguration _configuration;

        public CrearCuentaController(IConfiguration configuration, RegistroDAL registroDAL)
        {
            _configuration = configuration;
            _registroDAL = registroDAL;
        }

        public IActionResult CrearCuenta()
        {
            return View();
        }

        [HttpPost]
        public Respuesta<Usuario> RegistrarUsuario(string nombre, string apellido, string matricula, 
            string carrera, string email, string password)
        {
            var reply = new Respuesta<Usuario>();
            var conexion = _configuration.GetConnectionString("ConexionDB");

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(matricula) || string.IsNullOrWhiteSpace(carrera) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                reply.Ok = false;
                reply.Mensaje = "Todos los campos son obligatorios.";
                return reply;
            }

            // Verificar si la matrícula ya existe
            var verificacion = _registroDAL.VerificarMatriculaExistente(matricula, conexion);
            if (verificacion.Ok && verificacion.ValorRetorno)
            {
                reply.Ok = false;
                reply.Mensaje = "La matrícula ya está registrada.";
                return reply;
            }

            // Registrar usuario
            var respuesta = _registroDAL.RegistrarUsuario(nombre, apellido, matricula, carrera, email, password, conexion);

            if (respuesta.Ok && respuesta.ValorRetorno != null)
            {
                var u = respuesta.ValorRetorno;

                HttpContext.Session.SetString("Nombre", u.NombreCompleto ?? "");
                HttpContext.Session.SetString("Rol", u.RolId.ToString() ?? "");

                var ConfigCookie = new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(30),
                };

                Response.Cookies.Append("IdUsuario", Convert.ToString(u.Id), ConfigCookie);
            }

            return respuesta;
        }

        [HttpPost]
        public JsonResult VerificarMatricula(string matricula)
        {
            var conexion = _configuration.GetConnectionString("ConexionDB");
            var respuesta = _registroDAL.VerificarMatriculaExistente(matricula, conexion);

            return Json(new { existe = respuesta.ValorRetorno });
        }
    }
}
