using BiometricoZTK.Domain.Entities.Auth;
using BiometricoZTK.Domain.Interfaces;
using BiometricoZTK.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Infrastructure.Repositories.Auth
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbContext _context;
        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetUserByEmail(string email)
        {
            return await _context.Usuario.Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == email && u.Activo);
        }

        public async Task<bool> ExisteCorreoAsync(string email)
        {
            return await _context.Usuario.AnyAsync(u => u.Correo == email);
        }

        public async Task<bool> CrearUsuario(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);

            var valor = await _context.SaveChangesAsync();

            return valor > 0;
        }

        public async Task<Usuario> GetByIdAsync(long Id)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<bool> ActualizarPassword(long UsuarioId, string nuevoHash)
        {
            var usuario = await _context.Usuario.FindAsync(UsuarioId);

            if (usuario == null) return false;

            usuario.PasswordHash = nuevoHash;
            usuario.DebeCambiarContraseña = false;
            usuario.Updated = DateTime.Now;
            usuario.UpdatedBy = "System";

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExisteRolAsync(long rolId)
        {
            return await _context.Rol.AnyAsync(r => r.Id == rolId && r.Activo);
        }
    }
}
