using BiometricoZTK.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ActividadLog> ActividadLogs { get; set; }
    }
}
