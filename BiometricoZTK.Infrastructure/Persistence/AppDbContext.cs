using BiometricoZTK.Domain.Entities;
using BiometricoZTK.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<ActividadLog> ActividadLogs { get; set; }

        #region Auth
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Rol> Rol { get; set; }
        #endregion
    }
}
