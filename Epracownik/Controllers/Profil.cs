using Epracownik.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epracownik.Controllers
{
    public class Profil : Controller
    {
        private readonly AppDbContext db;
        public Profil(AppDbContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            var username_currect_user = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username_currect_user))
            {
                int id_currect_user = (int)HttpContext.Session.GetInt32("Session_id");
                using (var contex = db.Database.BeginTransaction())
                {
                    var user_info = db.InformacjePersonalnes.First(x => x.IdPracownika == id_currect_user);
                    return View(user_info);
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }
        }
    }
}
