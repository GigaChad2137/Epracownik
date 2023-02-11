using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string Role1 { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
