using BiometricoZTK.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(AuthRequest login);

        Task<AuthResponse> RegistrarUsuarioAsync(RegisterRequest register);

        Task<AuthResponse> CambiarPasswordAsync(long usuarioId, string passwordActual, string nuevaPassword);


    }
}
