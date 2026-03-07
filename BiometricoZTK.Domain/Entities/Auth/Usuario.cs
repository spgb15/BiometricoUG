using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Domain.Entities.Auth
{
    public class Usuario : EntityBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public long RolId { get; set; }
        public Rol? Rol { get; set; }
        public bool DebeCambiarContraseña { get; set; } = true;
    }
}
