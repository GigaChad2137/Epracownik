using Epracownik.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Threading.Tasks;
using iTextSharp.text.pdf.draw;

namespace Epracownik.Controllers
{
    public class Main : Controller
    {
        public class WnioskiDoRozpatrzeniaModel
        {
            public string TypWniosku { get; set; }
            public DateTime DataRozpoczecia { get; set; }
            public DateTime DataZakonczenia { get; set; }
            public string Notka { get; set; }
            public int? Kwota { get; set; }
            public string Status_Wniosku { get; set; }
        }
        public class Status_Pracy
        {
            public string Status_pracy { get; set; }
        }

        private readonly AppDbContext db;
        public Main(AppDbContext context)
        {
            db = context;
        } 

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "User")
            {
                var status_pracy = HttpContext.Session.GetString("Session_Praca");
                var model = new Status_Pracy
                {
                    Status_pracy = status_pracy
                };
                
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
            
        }

        public IActionResult Wnioski()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "User")
            {
                var id_currect_user = HttpContext.Session.GetInt32("Session_id");
                using (var contex = db.Database.BeginTransaction())
                {
                    var wnioski_do_rozpatrzenia = (from users in db.Users
                                                   join informacje_personalne in db.InformacjePersonalnes
                                                   on users.Id equals informacje_personalne.IdPracownika
                                                   join user_wnioski in db.UserWnioskis on users.Id equals user_wnioski.IdPracownika
                                                   join wnioski in db.Wnioskis on user_wnioski.IdWniosku equals wnioski.Id
                                                   where user_wnioski.StatusWniosku == null && users.Id == id_currect_user
                                                        select new
                                                        {
                                                            wnioski.TypWniosku,
                                                            user_wnioski.DataRozpoczecia,
                                                            user_wnioski.DataZakonczenia,
                                                            user_wnioski.Notka,
                                                            user_wnioski.Kwota,
                                                            user_wnioski.StatusWniosku
                                                        }).ToList();
                    List<WnioskiDoRozpatrzeniaModel> wnioski_uzytkownika = new List<WnioskiDoRozpatrzeniaModel>();
                    foreach(var wniosek in wnioski_do_rozpatrzenia)
                    {
                        string status = wniosek.StatusWniosku == true ? "Zaakceptowany" : (wniosek.StatusWniosku == null ? "Nierozpatrzony" : "Odrzucony");
                        wnioski_uzytkownika.Add(new WnioskiDoRozpatrzeniaModel { TypWniosku = wniosek.TypWniosku, DataRozpoczecia = wniosek.DataRozpoczecia, DataZakonczenia = wniosek.DataZakonczenia, Notka = wniosek.Notka, Kwota = wniosek.Kwota,Status_Wniosku=status });
                    }
                    return View(wnioski_uzytkownika);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
       
    }
}
