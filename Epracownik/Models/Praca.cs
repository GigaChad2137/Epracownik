using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class Praca
    {
        public int Id { get; set; }
        public int IdPracownika { get; set; }
        public DateTime Data { get; set; }
        public DateTime? DataRozpoczecia { get; set; }
        public DateTime? DataZakonczenia { get; set; }
        public string CzyPracuje { get; set; }

        public virtual User IdPracownikaNavigation { get; set; }
    }
}
