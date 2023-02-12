using Epracownik.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class Status_Praca : Controller
    {
        private readonly AppDbContext db;
        public Status_Praca(AppDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var id_currect_user = HttpContext.Session.GetInt32("Session_id");
                    DateTime thisDay = DateTime.Today;
                    var czy_pracuje = db.Pracas.First(x => x.IdPracownika == id_currect_user && x.Data == thisDay);
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
                    db.SaveChanges();
                    contex.Commit();
                }
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
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
    }
}
