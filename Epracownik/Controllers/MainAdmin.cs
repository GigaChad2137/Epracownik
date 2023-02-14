using Epracownik.Data;
using Epracownik.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
        public class Pracownicy_control
        {
            public string Imie { get; set; }
            public string Nazwisko { get; set; }
            public DateTime? DataRozpoczecia { get; set; }
            public DateTime? DataZakonczenia { get; set; }
            public string Status_Pracy { get; set; }
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

        public IActionResult Pracownicy()
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
                                                   join praca in db.Pracas on users.Id equals praca.IdPracownika
                                                   where praca.Data == DateTime.Today
                                                   select new
                                                   {
                                                       informacje_personalne.Imie,
                                                       informacje_personalne.Nazwisko,
                                                       praca.DataRozpoczecia,
                                                       praca.DataZakonczenia,
                                                       praca.CzyPracuje,
                                                   }).ToList();

                    List<Pracownicy_control> pracownicy_control = new List<Pracownicy_control>();
                    foreach (var wniosek in wnioski_do_rozpatrzenia)
                    {
                        pracownicy_control.Add(new Pracownicy_control { Imie = wniosek.Imie, Nazwisko = wniosek.Nazwisko, DataRozpoczecia = wniosek.DataRozpoczecia, DataZakonczenia = wniosek.DataZakonczenia, Status_Pracy=wniosek.CzyPracuje });
                    }
                    return View(pracownicy_control);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }

        [HttpPost]
        public IActionResult DodajPracownika(string username_konto,string password,string retype_password,string imie,string nazwisko,int reg_zarobki, bool czy_szef)
        {
            var username = HttpContext.Session.GetString("Session_Username");
            var rola = HttpContext.Session.GetString("Session_Rola");
            if (!string.IsNullOrEmpty(username) && rola == "Admin")
            {
                byte[] Source;
                byte[] hashed_Data;
                var reg_user = username_konto;
                var reg_passwd = password;
                var reg_retype_passwd = retype_password;
                var reg_imie = imie;
                var reg_nazwisko = nazwisko;
                DateTime today = DateTime.Today;
              
                using (var contex = db.Database.BeginTransaction())
                {
                    if (db.Users.Where(c => c.Username == reg_user).Count() > 0)
                    {
                    ViewData["Message"] = "Podany użytkownik już istnieje";
                    return View();
                    }
                    else
                    {
                        if (reg_user != "" && reg_user.Length >= 4)
                        {
                            if (reg_passwd == reg_retype_passwd && reg_passwd != "" && reg_passwd.Length > 4 && reg_passwd.Length < 20 && reg_zarobki > 0 && reg_imie != "" && reg_nazwisko != "")
                            {
                                Source = ASCIIEncoding.ASCII.GetBytes(reg_passwd);
                                hashed_Data = new MD5CryptoServiceProvider().ComputeHash(Source);
                                string passwd_hash = Convert.ToBase64String(hashed_Data);
                                User new_usr = new User { Username = reg_user, Password = passwd_hash };
                                db.Users.Add(new_usr);
                                if (czy_szef == true)
                                {
                                    db.UserRoles.Add(new UserRole { IdUser = new_usr.Id, IdRole = 1 });
                                }
                                else
                                {
                                    db.UserRoles.Add(new UserRole { IdUser = new_usr.Id, IdRole = 2 });
                                }
                                db.InformacjePersonalnes.Add(new InformacjePersonalne { IdPracownika = new_usr.Id, Imie = reg_imie, Nazwisko = reg_nazwisko, Zarobki = reg_zarobki, DniUrlopowe = 30, DataZatrudnienia = today });
                                db.SaveChanges();
                                contex.Commit();
                                ViewData["Message"] = "Dodano Pracownika";
                                return View();
                            }
                            else if (reg_passwd == "")
                            {
                                ViewData["Message"] = "Hasło musi zawierać się między 4-20 znaków";
                                return View();
                            }
                            else if (reg_passwd.Length < 4 || reg_passwd.Length > 20)
                            {
                                ViewData["Message"] = "Hasło musi zawierać się między 4-20 znaków";
                                return View();
                            }
                            else if (reg_passwd != reg_retype_passwd)
                            {
                                ViewData["Message"] = "Hasła nie są identyczne";
                                return View();
                            }
                            else
                            {
                                ViewData["Message"] = "Niepoprawne Dane";
                                return View();
                            }
                        }
                        else if (reg_user.Length <= 4 || reg_user.Length >= 20)
                        {
                            ViewData["Message"] = "Użytkownik musi zawierać się między 4-20 znaków";
                            return View();
                        }
                        else
                        {
                            ViewData["Message"] = "Uzupełnij Wszystkie Dane!";
                            return View();
                        }
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie masz uprawnień!" });
            }
        }

    }
}
