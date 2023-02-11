using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class User
    {
        public User()
        {
            Pracas = new HashSet<Praca>();
            UserWnioskis = new HashSet<UserWnioski>();
            WiadomosciIdNadawcyNavigations = new HashSet<Wiadomosci>();
            WiadomosciIdOdbiorcyNavigations = new HashSet<Wiadomosci>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual InformacjePersonalne InformacjePersonalne { get; set; }
        public virtual UserRole UserRole { get; set; }
        public virtual ICollection<Praca> Pracas { get; set; }
        public virtual ICollection<UserWnioski> UserWnioskis { get; set; }
        public virtual ICollection<Wiadomosci> WiadomosciIdNadawcyNavigations { get; set; }
        public virtual ICollection<Wiadomosci> WiadomosciIdOdbiorcyNavigations { get; set; }
    }
}
