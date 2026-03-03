using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Domain.Entities.Auth
{
    public class Rol : EntityBase
    {
        public string Descripcion { get; set; } = string.Empty;
    }
}
