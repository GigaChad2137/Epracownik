using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class Wnioski
    {
        public Wnioski()
        {
            UserWnioskis = new HashSet<UserWnioski>();
        }

        public int Id { get; set; }
        public string TypWniosku { get; set; }

        public virtual ICollection<UserWnioski> UserWnioskis { get; set; }
    }
}
