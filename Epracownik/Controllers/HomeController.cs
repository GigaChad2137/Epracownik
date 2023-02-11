using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Epracownik.Data;
using System.Web;
using Microsoft.AspNetCore.Http;
using Epracownik.Models;
using System.Diagnostics;

namespace Epracownik.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;
        public HomeController(AppDbContext context)
        {
            db = context;
        }

        public IActionResult Index(string Message)
        {
            ViewData["Message"] = Message;
            return View();
        }
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            using (var contex = db.Database.BeginTransaction())
            {
                var passwdhashed = GetHashedText(password);
                if (db.Users.Where(c => c.Username == username && c.Password == passwdhashed).Any())
                {
                    string status_pracy = "Zakończ Prace";
                    var id_finder = from c in db.Users where c.Username == username select c; // zapytanie do bazy danych zwracające wszystkie rekordy pasujące do wymagań
                    var id_checker = id_finder.FirstOrDefault<User>();  //funkcja zwracająca pierwszy wiersz z wcześniejszego zapytania
                    DateTime thisDay = DateTime.Today; //zmienna zawierająca aktualną date
                    var czy_pracuje = db.Pracas.Where(x => x.IdPracownika == id_checker.Id && x.Data == thisDay).Count(); // zapytanie do bazy danych, które zlicza ilość zwróconych rekordów
                    Console.WriteLine(czy_pracuje);
                    if (czy_pracuje == 0)  // jeżeli zapytanie zwróci 0 to wykona się zapisanie rekordu i commit transakcji - czyli dodanie rekordu do bazy danych
                    {
                        db.Pracas.Add(new Praca { IdPracownika = id_checker.Id, Data = thisDay, DataRozpoczecia = null, DataZakonczenia = null, CzyPracuje = "Nie Pracuje" });
                        db.SaveChanges();
                        status_pracy = "Rozpocznij Prace";
                    }
                    else
                    {
                        var stan_pracy_check = db.Pracas.Where(x => x.IdPracownika == id_checker.Id && x.Data == thisDay).First();
                        if (stan_pracy_check.CzyPracuje == "Nie Pracuje")
                        {
                            status_pracy = "Rozpocznij Prace";
                        }
                    }
                    HttpContext.Session.SetString("Session_Username", username);
                    HttpContext.Session.SetInt32("Session_id", id_checker.Id);
                    HttpContext.Session.SetString("Session_Praca", status_pracy);
                    contex.Commit();
                    return RedirectToAction("index", "Main");

                }
                else
                {


                    ViewData["Message"] = "Nieprawidłowy login lub hasło";
                    return View();
                }
            }
                
        }
        private string GetHashedText(string inputData) //funkcja hashująca 
        {
            byte[] tmpSource;
            byte[] tmpData;
            tmpSource = ASCIIEncoding.ASCII.GetBytes(inputData); //przekonwertowuje do typu byte 
            tmpData = new MD5CryptoServiceProvider().ComputeHash(tmpSource); // użycie gotowej funkcji, która hashuje przekazaną tablice
            return Convert.ToBase64String(tmpData); //zahashowaną tablice przekonwertowywuje do stringa i następnia zwraca
        }
    }
 
}
