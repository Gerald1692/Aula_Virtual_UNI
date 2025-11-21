using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
//Loginn?
namespace Aula_Virtual_UNI.Controllers
{
    public class LoginController : Controller
    {
        private readonly Login _LoginDAL;
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration, Login login)
        {
            _LoginDAL = login;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public Respuesta<Usuario> IniciarSesion(string Usuario, string Contrasena)
        {
            var reply = new Respuesta<Usuario>();
            var conexion = _configuration.GetConnectionString("ConexionDB");

            var respuesta = _LoginDAL.IniciarSesion(Usuario, Contrasena, conexion);

            if (respuesta.Ok && respuesta.ValorRetorno != null)
            {
                var u = respuesta.ValorRetorno;

                HttpContext.Session.SetString("Nombre", u.NombreUsuario ?? "");
                HttpContext.Session.SetString("Rol", u.Rol ?? "");

                var ConfigCookie = new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(30),

                };

                Response.Cookies.Append("IdUsuario", Convert.ToString(u.IdUsuario), ConfigCookie);

            }

            return respuesta;
        }

    }
}
