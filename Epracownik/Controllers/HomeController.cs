﻿using Microsoft.AspNetCore.Mvc;
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

namespace Epracownik.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var passwdhashed = GetHashedText(password);
            if (_context.Users.Where(c => c.Username == username && c.Password == passwdhashed).Count() > 0)
            {
                var id_finder = from c in _context.Users where c.Username == username select c; // zapytanie do bazy danych zwracające wszystkie rekordy pasujące do wymagań
                var id_checker = id_finder.FirstOrDefault<User>();  //funkcja zwracająca pierwszy wiersz z wcześniejszego zapytania
                HttpContext.Session.SetString("Session_Username", username);
                HttpContext.Session.SetInt32("Session_id", id_checker.Id);
                return RedirectToAction("index", "Main");
            }
            else
            {
                

                ViewData["Message"] = "Nieprawidłowy login lub hasło";
                return View();
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