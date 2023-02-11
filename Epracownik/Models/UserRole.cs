using System;
using System.Collections.Generic;

#nullable disable

namespace Epracownik.Models
{
    public partial class UserRole
    {
        public int IdUser { get; set; }
        public int? IdRole { get; set; }

        public virtual Role IdRoleNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
    }
}
