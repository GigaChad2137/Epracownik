using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class Logout : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {

                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home", new { Message = "Zostałeś Wylogowany" });
            }
            return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
        }
    }
}
