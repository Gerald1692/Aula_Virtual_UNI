using Microsoft.AspNetCore.Mvc;

namespace Aula_Virtual_UNI.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
