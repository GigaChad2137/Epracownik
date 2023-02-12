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
        public IActionResult Index(string Message)
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username)) {
                var wnioski = db.Wnioskis.ToList();
                ViewData["Message"] = Message;
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

            var username_currect_user = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username_currect_user))
            {
                string message = "Brak wymaganego Pola"; 
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
                            if (wage != null)
                            {
                                db.UserWnioskis.Add(new UserWnioski { IdPracownika = id_currect_user, IdWniosku = typ_wniosku, DataRozpoczecia = DateTime.Today, DataZakonczenia = DateTime.Today, Notka = note, Kwota = Convert.ToInt32(wage) });
                                db.SaveChanges();
                                message = "Wniosek Wysłany!";
                            }
                            else
                            {
                                message = "Brak Wymaganego Pola";
                            }

                        }
                        else
                        {
                            if (start_date == DateTime.MinValue || end_date == DateTime.MinValue || start_date > end_date)
                            {

                                message = "Wybierz Poprawny Zakres Dat!";
                            }
                            else
                            {
                                db.UserWnioskis.Add(new UserWnioski { IdPracownika = id_currect_user, IdWniosku = typ_wniosku, DataRozpoczecia = start_date, DataZakonczenia = end_date, Notka = note });
                                db.SaveChanges();
                               message= "Wniosek Wysłany!";
                            }
                        }
                        contex.Commit();
                        }
                    
                }
                return RedirectToAction("Index", "AddWniosek", new { Message = message });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
    }
}
