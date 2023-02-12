using Epracownik.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epracownik.Controllers
{
    public class EwidencjaPracy : Controller
    {
        private readonly AppDbContext db;
        public EwidencjaPracy(AppDbContext context)
        {
            db = context;
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
        public IActionResult Index()
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
                    PdfPTable table_SUM = new PdfPTable(3);
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
