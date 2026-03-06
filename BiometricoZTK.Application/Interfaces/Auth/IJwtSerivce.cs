using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Interfaces.Auth
{
    public interface IJwtSerivce
    {
        string GenerarToken(string email, long rol, string nombre, long userId);
    }
}
