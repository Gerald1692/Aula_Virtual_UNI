using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Aula_Virtual_UNI.Controllers
{
    public class MenuController : Controller
    {
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
