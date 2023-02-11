using Epracownik.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class Main : Controller
    {
        private readonly AppDbContext _context;
        public Main(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                var status_pracy = HttpContext.Session.GetString("Session_Username");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }
            
        }
        public IActionResult Praca()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            
            if (!string.IsNullOrEmpty(username))
            {
                var id_currect_user = HttpContext.Session.GetInt32("Session_id");
                DateTime thisDay = DateTime.Today;
                var czy_pracuje = _context.Pracas.First(x => x.IdPracownika == id_currect_user && x.Data == thisDay);
                DateTime Date_with_time = DateTime.Now;
                if (czy_pracuje.CzyPracuje == "Pracuje")
                {
                    HttpContext.Session.SetString("Session_Praca", "Rozpocznij Prace");
                    czy_pracuje.DataZakonczenia = Date_with_time;
                    czy_pracuje.CzyPracuje = "Nie Pracuje";

                }
                else if (czy_pracuje.CzyPracuje == "Nie Pracuje")
                {
                    HttpContext.Session.SetString("Session_Praca", "Zakończ Prace");
                    czy_pracuje.DataRozpoczecia = Date_with_time;
                    czy_pracuje.CzyPracuje = "Pracuje";
                }
                _context.SaveChanges();
                return RedirectToAction("Index", "Main");
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }

        public IActionResult Wnioski()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
        public IActionResult Urlopy()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
    }
}
