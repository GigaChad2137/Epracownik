using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class InformacjePersonalne
    {
        public int IdPracownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int Zarobki { get; set; }
        public int DniUrlopowe { get; set; }
        public DateTime DataZatrudnienia { get; set; }

        public virtual User IdPracownikaNavigation { get; set; }
    }
}
