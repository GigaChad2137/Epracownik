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
        public IActionResult Urlopy()
        {
            var username = HttpContext.Session.GetString("Session_Username");
            if (!string.IsNullOrEmpty(username))
            {
                using (var memoryStream = new MemoryStream())
                {
                    List<Pdf_view> pdfView = new List<Pdf_view>();
                    pdfView.Add(new Pdf_view { data = "01.01.2023", czas_start = new TimeSpan(9, 0, 0), czas_stop = new TimeSpan(17, 0, 0), godziny = "8", kwota = "320" });
                    pdfView.Add(new Pdf_view { data = "02.01.2023", czas_start = new TimeSpan(9, 0, 0), czas_stop = new TimeSpan(17, 0, 0), godziny = "8", kwota = "320" });
                    pdfView.Add(new Pdf_view { data = "03.01.2023", czas_start = new TimeSpan(9, 0, 0), czas_stop = new TimeSpan(17, 0, 0), godziny = "8", kwota = "320" });

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

                    var cellTitleRight = new PdfPCell(new Phrase("Podpis prezesa\n\n..............\n\nData\n..............", fontContent));
                    cellTitleRight.Border = Rectangle.NO_BORDER;
                    cellTitleRight.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellTitleRight.PaddingTop = 20f;
                    titleTable.AddCell(cellTitleRight);

                    document.Add(titleTable);

                    var headerTable = new PdfPTable(1);
                    headerTable.TotalWidth = 500f;

                    var cellHeader = new PdfPCell(new Phrase("Firma S.A. Rozliczenie za okres: Miesiąc rozliczenia", fontHeader));
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
                    headerCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(headerCell);
               
                    headerCell = new PdfPCell(new Phrase("Rozpoczęcie", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Zakończenie", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Godziny", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(headerCell);

                    headerCell = new PdfPCell(new Phrase("Razem", fontTableHeader));
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Border = Rectangle.NO_BORDER;
                    headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(headerCell);

                    // Dopisanie danych do tabeli
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

                    Paragraph date = new Paragraph(DateTime.Now.ToString("dd.MM.yyyy"), fontContent);

                    Paragraph signatureRight = new Paragraph("Wynagrodzenie: 4000 euro", fontContent);

                    PdfPTable table_SUM= new PdfPTable(3);
                    table.WidthPercentage = 100;

                    PdfPCell cell1 = new PdfPCell(signatureLeft);
                    cell1.Border = Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    table_SUM.AddCell(cell1);

                    PdfPCell cell2 = new PdfPCell(date);
                    cell2.Border = Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table_SUM.AddCell(cell2);

                    PdfPCell cell3 = new PdfPCell(signatureRight);
                    cell3.Border = Rectangle.NO_BORDER;
                    cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table_SUM.AddCell(cell3);

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
