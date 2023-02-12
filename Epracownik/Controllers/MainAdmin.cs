using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Epracownik.Controllers.Main;

namespace Epracownik.Controllers
{
    public class MainAdmin : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "Admin")
            {
                var status_pracy = HttpContext.Session.GetString("Session_Praca");
                var model = new Status_Pracy
                {
                    Status_pracy = status_pracy
                };
                Console.WriteLine(model);
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
    }
}
