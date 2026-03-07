using BiometricoZTK.Domain.Entities.Auth;
using BiometricoZTK.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK.Infrastructure.Seeding
{
    public static class RolSeeder
    {
        public const string AdminRolDescripcion = "Admin";

        public static async Task SeedAdminRolAsync(AppDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var existeAdmin = await context.Rol
                .AnyAsync(r => r.Descripcion == AdminRolDescripcion);

            if (existeAdmin)
                return;

            context.Rol.Add(new Rol
            {
                Descripcion = AdminRolDescripcion,
                Activo = true,
                Created = DateTime.UtcNow,
                CreatedBy = "Seed"
            });

            await context.SaveChangesAsync();
        }
    }
}
