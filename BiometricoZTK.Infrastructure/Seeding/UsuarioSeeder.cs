using BiometricoZTK.Domain.Entities.Auth;
using BiometricoZTK.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK.Infrastructure.Seeding
{
    public static class UsuarioSeeder
    {
        public const string DefaultUserEmail = "test@test.com";
        private const string DefaultUserPassword = "Test2024!";
        private const string DefaultUserName = "Test";

        public static async Task SeedDefaultUserAsync(
            AppDbContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (passwordHasher == null)
                throw new ArgumentNullException(nameof(passwordHasher));

            var existeUsuario = await context.Usuario
                .AnyAsync(u => u.Correo == DefaultUserEmail);

            if (existeUsuario)
                return;

            var adminRol = await context.Rol
                .FirstOrDefaultAsync(r => r.Descripcion == RolSeeder.AdminRolDescripcion && r.Activo);

            if (adminRol == null)
                return;

            var usuario = new Usuario
            {
                Nombre = DefaultUserName,
                Correo = DefaultUserEmail,
                RolId = adminRol.Id,
                Rol = adminRol,
                Activo = true,
                DebeCambiarContraseña = false,
                Created = DateTime.UtcNow,
                CreatedBy = "Seed"
            };

            usuario.PasswordHash = passwordHasher.HashPassword(usuario, DefaultUserPassword);

            context.Usuario.Add(usuario);
            await context.SaveChangesAsync();
        }
    }
}
