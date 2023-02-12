using Microsoft.AspNetCore.Mvc;

namespace Epracownik.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
    }
}
