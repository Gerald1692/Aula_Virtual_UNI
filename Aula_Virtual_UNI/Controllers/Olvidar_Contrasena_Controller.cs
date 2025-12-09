using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Aula_Virtual_UNI.Controllers
{
    public class Olvidar_Contrasena_Controller : Controller
    {
        private readonly Login _LoginDAL;
        private readonly IConfiguration _configuration;

        public Olvidar_Contrasena_Controller(IConfiguration configuration, Login login)
        {
            _LoginDAL = login;
            _configuration = configuration;
        }

        public IActionResult Olvidar_Contrasena()
        {
            return View();
        }

        [HttpPost]
        public Respuesta<string> RecuperarContrasena(string Usuario)
        {
            var conexion = _configuration.GetConnectionString("ConexionDB");
            var respuesta = _LoginDAL.RecuperarContrasena(Usuario, conexion);
            return respuesta;
        }
    }
}
