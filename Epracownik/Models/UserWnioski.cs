using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class UserWnioski
    {
        public int Id { get; set; }
        public int IdPracownika { get; set; }
        public int IdWniosku { get; set; }
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }
        public string Notka { get; set; }
        public int? Kwota { get; set; }
        public bool? StatusWniosku { get; set; }
        public int NotiC { get; set; }

        public virtual User IdPracownikaNavigation { get; set; }
        public virtual Wnioski IdWnioskuNavigation { get; set; }
    }
}
