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
        public JsonResult RegistrarUsuario(string nombre, string apellido1, string apellido2, 
            string cedula, string telefono, string correo, string sede, string password)
        {
            var reply = new Respuesta<Usuario>();
            var conexion = _configuration.GetConnectionString("ConexionDB");

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido1) ||
                string.IsNullOrWhiteSpace(apellido2) || string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(sede) || string.IsNullOrWhiteSpace(password))
            {
                reply.Ok = false;
                reply.Mensaje = "Todos los campos son obligatorios.";
                return Json(reply);
            }

            // Verificar si la cédula ya existe
            var verificacion = _registroDAL.VerificarCedulaExistente(cedula, conexion);
            if (verificacion.Ok && verificacion.ValorRetorno)
            {
                reply.Ok = false;
                reply.Mensaje = "La cédula ya está registrada.";
                return Json(reply);
            }

            // Registrar usuario
            var respuesta = _registroDAL.RegistrarUsuario(nombre, apellido1, apellido2, cedula, 
                telefono, correo, sede, password, conexion);

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

            return Json(respuesta);
        }

        [HttpPost]
        public JsonResult VerificarCedula(string cedula)
        {
            var conexion = _configuration.GetConnectionString("ConexionDB");
            var respuesta = _registroDAL.VerificarCedulaExistente(cedula, conexion);

            return Json(new { existe = respuesta.ValorRetorno });
        }
    }
}
