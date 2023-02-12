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
            if (!string.IsNullOrEmpty(username))
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
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }
            
        }
        public IActionResult Praca()
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
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }

        }
        public class Pdf_view
        {
            public string data { get; set; }
            public TimeSpan? czas_start { get; set; }
            public TimeSpan? czas_stop { get; set; }
            public string godziny { get; set; }
            public string kwota { get; set; }
        }

        public (List<Pdf_view>, string, string) fillList()
        {
            List<Pdf_view> items = new List<Pdf_view>();
            var id_currect_user = HttpContext.Session.GetInt32("Session_id");
            var today = DateTime.Today;
            string suma_wyplata = "...";
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);
            string data_rozliczenia = $"{first.ToShortDateString()}-{last.ToShortDateString()}";

            double suma_miesiac = 0;

            using (var contex = db.Database.BeginTransaction())
            {
                var dane_usera = db.InformacjePersonalnes.Where(x => x.IdPracownika == id_currect_user).First();
                double zarobek_na_godzine = dane_usera.Zarobki / 160;
                var miesiac_rozliczenia = db.Pracas.Where(x => x.IdPracownika == id_currect_user && x.Data >= first && x.Data <= last && x.DataRozpoczecia != null && x.DataZakonczenia != null).ToList();
                foreach (var dzien in miesiac_rozliczenia)
                {
                    double suma_dzien = 0;
                    TimeSpan? godziny_przepracowane = dzien.DataZakonczenia - dzien.DataRozpoczecia;
                    suma_dzien = Math.Round(godziny_przepracowane.Value.TotalHours * zarobek_na_godzine, 2);
                    items.Add(new Pdf_view { data = dzien.Data.ToShortDateString(), czas_start = dzien.DataRozpoczecia.Value.TimeOfDay, czas_stop = dzien.DataZakonczenia.Value.TimeOfDay, godziny = $"{godziny_przepracowane.Value.Hours}h {godziny_przepracowane.Value.Minutes}min", kwota = $"{suma_dzien}$" });
                    suma_miesiac = suma_miesiac + suma_dzien;
                }
                suma_wyplata = $"{suma_miesiac}$";
            }
            return (items, suma_wyplata, data_rozliczenia);


        }
        public IActionResult Urlopy()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var (pdfView, suma_wynagrodzenie, data_rozliczenia) = fillList();
                    // Dopisanie danych do tabeli
                   

                    // Utworzenie dokumentu PDF
                    Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    var fontHeader = FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD);
                    var fontContent = FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.NORMAL);

                    var titleTable = new PdfPTable(2);
                    titleTable.TotalWidth = 500f;
                    titleTable.SetWidths(new float[] { 250f, 250f });

                    var cellTitleLeft = new PdfPCell(new Phrase("Firma S.A.", fontTitle));
                    cellTitleLeft.Border = Rectangle.NO_BORDER;
                    cellTitleLeft.PaddingTop = 20f;
                    titleTable.AddCell(cellTitleLeft);

                    var cellTitleRight = new PdfPCell(new Phrase("Podpis prezesa\n\n....................\n\nData\n....................", fontContent));
                    cellTitleRight.Border = Rectangle.NO_BORDER;
                    cellTitleRight.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellTitleRight.PaddingTop = 20f;
                    titleTable.AddCell(cellTitleRight);

                    document.Add(titleTable);

                    var headerTable = new PdfPTable(1);
                    headerTable.TotalWidth = 500f;

                    var cellHeader = new PdfPCell(new Phrase($"Firma S.A. Rozliczenie za okres: {data_rozliczenia}", fontHeader));
                    cellHeader.Border = Rectangle.NO_BORDER;
                    cellHeader.PaddingTop = 40f;

                    headerTable.AddCell(cellHeader);
                    document.Add(headerTable);
                    document.Add(new Paragraph("\n"));
                    document.Add(new Paragraph("\n"));
                    // Dodawanie tabeli z danymi do dokumentu PDF
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;

                    // Dodawanie nagłówków tabeli
                    Font fontTableHeader = FontFactory.GetFont("Arial", 10, Font.BOLD);
                    var headerCell = new PdfPCell(new Phrase("Data", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(headerCell);
               
                    headerCell = new PdfPCell(new Phrase("Rozpoczęcie", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Zakończenie", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Godziny", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Razem", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(headerCell);

                   
                    foreach (var item in pdfView)
                    {
                        table.AddCell(item.data);
                        table.AddCell(item.czas_start.Value.ToString(@"hh\:mm"));
                        table.AddCell(item.czas_stop.Value.ToString(@"hh\:mm"));
                        table.AddCell(item.godziny);
                        table.AddCell(item.kwota);
                    }

                    document.Add(table);
                    document.Add(new Paragraph("\n"));
                    Paragraph signatureLeft = new Paragraph("Podpis", fontContent);

                    Paragraph date = new Paragraph("Data", fontContent);

                    Paragraph signatureRight = new Paragraph("Wynagrodzenie", fontContent);

                    PdfPTable table_SUM= new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.PaddingTop = 200;



                    PdfPCell cell_podpis = new PdfPCell(signatureLeft);
                    cell_podpis.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell_podpis);

                    PdfPCell cell_data = new PdfPCell(date);
                    cell_data.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell_data);

                    PdfPCell cell_wynagrodzenie = new PdfPCell(signatureRight);
                    cell_wynagrodzenie.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell_wynagrodzenie);

                    table_SUM.AddCell("");

                    PdfPCell cell_data1 = new PdfPCell(new Paragraph(DateTime.Now.ToString("dd.MM.yyyy"), fontContent));
                    cell_data1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell_data1);

                    PdfPCell cell_wynagrodzenie1 = new PdfPCell(new Paragraph(suma_wynagrodzenie, fontContent));
                    cell_wynagrodzenie1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell_wynagrodzenie1);




                   

                    document.Add(table_SUM);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "Rozliczenie.pdf");

                }
            }
            else
            {
                return RedirectToAction("Index", "Home", new { Message = "Nie jesteś zalogowany!" });
            }
           

        }
       
    }
}
