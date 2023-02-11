using Epracownik.Data;
using Epracownik.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class AddWniosek : Controller
    {
        private readonly AppDbContext db;
        public AddWniosek(AppDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username)) {
                var wnioski = db.Wnioskis.ToList();
                return View(wnioski);
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }
        }
        [HttpPost]
        public IActionResult Index(string select_wnioski, DateTime start_date,DateTime end_date,string wage,string note)
        {
            Console.WriteLine(select_wnioski);
            Console.WriteLine(start_date);
            Console.WriteLine(end_date);
            Console.WriteLine(wage);
            Console.WriteLine(note);

            var username_currect_user = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username_currect_user))
            {
                if (select_wnioski != null)
                {
                    int typ_wniosku = int.Parse(select_wnioski);
                    int id_currect_user = (int)HttpContext.Session.GetInt32("Session_id");
                    string tresc_wiadomosci = note;
                  
                    using (var contex = db.Database.BeginTransaction())
                    {
                        var testow = db.Wnioskis.First(x => x.Id == typ_wniosku);
                        if (testow.TypWniosku == "Wynagrodzenie")
                        {
                            if (wage != "")
                            {
                                db.UserWnioskis.Add(new UserWnioski { IdPracownika = id_currect_user, IdWniosku = typ_wniosku, DataRozpoczecia = DateTime.Today, DataZakonczenia = DateTime.Today, Notka = note, Kwota = Convert.ToInt32(wage) });
                                db.SaveChanges();
                                ViewData["Message"] = "Wniosek Wysłany!";
                            }
                            else
                            {
                                ViewData["Message"] = "Brak wymaganego Pola";
                            }

                        }
                        else
                        {
                            if (start_date == null || end_date == null || start_date > end_date)
                            {

                                ViewData["Message"] = "Brak wymaganego Pola";
                            }
                            else
                            {
                                db.UserWnioskis.Add(new UserWnioski { IdPracownika = id_currect_user, IdWniosku = typ_wniosku, DataRozpoczecia = start_date, DataZakonczenia = end_date, Notka = note });
                                db.SaveChanges();
                                ViewData["Message"] = "Wniosek Wysłany!";
                            }
                        }
                        contex.Commit();
                        }
                    
                }
                return RedirectToAction("Index", "AddWniosek");
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
    }
}
