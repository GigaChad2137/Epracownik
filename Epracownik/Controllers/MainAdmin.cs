using Epracownik.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Epracownik.Controllers
{
    public class MainAdmin : Controller
    {
        public class Status_Pracy
        {
            public string Status_pracy { get; set; }
        }

        private readonly AppDbContext db;
        public MainAdmin(AppDbContext context)
        {
            db = context;
        }
        public class WnioskiDoRozpatrzeniaModel
        {
            public int Id_wniosku { get; set; }
            public string TypWniosku { get; set; }
            public int DniUrlopowe { get; set; }
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public DateTime DataRozpoczecia { get; set; }
            public DateTime DataZakonczenia { get; set; }
            public string Notka { get; set; }
            public int? Kwota { get; set; }
        }
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
            if (!string.IsNullOrEmpty(username) && rola == "Admin")
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var wnioski_do_rozpatrzenia = (from users in db.Users
                                                   join informacje_personalne in db.InformacjePersonalnes
                                                   on users.Id equals informacje_personalne.IdPracownika
                                                   join user_wnioski in db.UserWnioskis on users.Id equals user_wnioski.IdPracownika
                                                   join wnioski in db.Wnioskis on user_wnioski.IdWniosku equals wnioski.Id
                                                   where user_wnioski.StatusWniosku == null
                                                   select new
                                                   {
                                                       user_wnioski.Id,
                                                       wnioski.TypWniosku,
                                                       informacje_personalne.Imie,
                                                       informacje_personalne.Nazwisko,
                                                       informacje_personalne.DniUrlopowe,
                                                       user_wnioski.DataRozpoczecia,
                                                       user_wnioski.DataZakonczenia,
                                                       user_wnioski.Notka,
                                                       user_wnioski.Kwota,
                                                   }).ToList();

                    List<WnioskiDoRozpatrzeniaModel> wnioski_uzytkownika = new List<WnioskiDoRozpatrzeniaModel>();
                    foreach (var wniosek in wnioski_do_rozpatrzenia)
                    {
                        wnioski_uzytkownika.Add(new WnioskiDoRozpatrzeniaModel { Id_wniosku = wniosek.Id, TypWniosku = wniosek.TypWniosku, Imie = wniosek.Imie, Nazwisko = wniosek.Nazwisko, DniUrlopowe = wniosek.DniUrlopowe, DataRozpoczecia = wniosek.DataRozpoczecia, DataZakonczenia = wniosek.DataZakonczenia, Notka = wniosek.Notka, Kwota = wniosek.Kwota });
                    }
                    return View(wnioski_uzytkownika);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
        [HttpGet("mainadmin/odrzuc_wniosek/{typ_wniosku}/{id}")]
        public IActionResult odrzuc_wniosek(string typ_wniosku, int id)
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "Admin")
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var wybrany_wniosek = db.UserWnioskis.Where(x => x.Id == id).First();
                    wybrany_wniosek.StatusWniosku = false;
                    db.SaveChanges();
                    contex.Commit();

                }
                return RedirectToAction("Wnioski");
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
        [HttpGet("mainadmin/akceptuj_wniosek/{typ_wniosku}/{id}")]
        public IActionResult akceptuj_wniosek(string typ_wniosku,int id)
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "Admin")
            {
                using (var contex = db.Database.BeginTransaction())
                {
                    var wybrany_wniosek = db.UserWnioskis.Where(x => x.Id == id).First();
                    var id_currect_user = HttpContext.Session.GetInt32("Session_id");
                    var uzytkownik_wnioskujacy = db.InformacjePersonalnes.Where(x => x.IdPracownika == id_currect_user).First();
                    if (typ_wniosku == "Urlop")
                    {
                        TimeSpan difference = wybrany_wniosek.DataZakonczenia - wybrany_wniosek.DataRozpoczecia;
                        int dni_roznica = uzytkownik_wnioskujacy.DniUrlopowe - difference.Days -1;

                        uzytkownik_wnioskujacy.DniUrlopowe = dni_roznica;
                        wybrany_wniosek.StatusWniosku = true;
                        db.SaveChanges();


                    }
                    else if (typ_wniosku == "L4")
                    {
                        wybrany_wniosek.StatusWniosku = true;
                        db.SaveChanges();
                    }
                    else if (typ_wniosku == "Wynagrodzenie")
                    {
                        uzytkownik_wnioskujacy.Zarobki = (int)wybrany_wniosek.Kwota;
                        wybrany_wniosek.StatusWniosku = true;
                        db.SaveChanges();
                    }

                    
                    contex.Commit();

                }
                return RedirectToAction("Wnioski");
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }
    }
}
