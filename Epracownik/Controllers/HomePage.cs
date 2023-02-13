using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class HomePage : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                var rola = HttpContext.Session.GetString("Session_Rola");
                if (rola == "Admin")
                {
                    HttpContext.Session.SetString("Session_Rola", "Admin");
                    return RedirectToAction("index", "MainAdmin");
                }
                else
                {
                    HttpContext.Session.SetString("Session_Rola", "User");
                    return RedirectToAction("index", "Main");
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
    }
}
