using BiometricoZTK.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Domain.Interfaces
{
    public interface ILoginRepository
    {
        Task<Usuario> GetUserByEmail(string email);
        Task<bool> ExisteCorreoAsync(string email);
        Task<bool> CrearUsuario(Usuario usuario);
        Task<bool> ActualizarPassword(long UsuarioId, string nuevoHash);

        Task<Usuario> GetByIdAsync(long Id);

        Task<bool> ExisteRolAsync(long rolId);
    }
}
