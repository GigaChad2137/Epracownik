using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class Wiadomosci
    {
        public int Id { get; set; }
        public int IdNadawcy { get; set; }
        public int IdOdbiorcy { get; set; }
        public string Wiadomosc { get; set; }
        public bool? CzyPrzeczytane { get; set; }

        public virtual User IdNadawcyNavigation { get; set; }
        public virtual User IdOdbiorcyNavigation { get; set; }
    }
}
