using AulaVirtualDAL;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

            Respuesta<Usuario> reply = new Respuesta<Usuario>();

            try
            {
                var Conexion = _configuration.GetConnectionString("ConexionDB");
                var respuesta = _LoginDAL.IniciarSesion(Usuario, Contrasena, Conexion);

                if (respuesta.Ok)
                {
                    reply = respuesta;

                    var Cookie = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddMinutes(30)
                    };

                    //Response.Cookies.Append("NombreUsuario", respuesta.Usuarios[0].NombreCompleto, Cookie);


                }
                else
                {
                    reply = respuesta;


                }

            }
            catch (Exception ex)
            {
                reply.Ok = false;
                reply.Mensaje = ex.Message;

            }


            return reply;


        }


    }
}
