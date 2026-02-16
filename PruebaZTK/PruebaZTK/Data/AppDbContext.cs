using Microsoft.EntityFrameworkCore;
using PruebaZTK.Models;

namespace PruebaZTK.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AsistenciaLog> Asistencias { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=AsistenciaDB;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}