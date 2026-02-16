using BiometricoZTK.Domain.Entities;
using BiometricoZTK.Domain.Interfaces;
using BiometricoZTK.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;
        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ActividadLog> logs)
        {
            await _context.ActividadLogs.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ActividadLog>> GetExistingLogAsync(string sn, DateTime inicio, DateTime fin)
        {
            return await _context.ActividadLogs
                .Where(x => x.DispositivoSN == sn && x.FechaHora >= inicio && x.FechaHora <= fin)
                .ToListAsync();
        }

        public async Task<DateTime?> GetLastLogDateAsync(string serialNumber)
        {
            return await _context.ActividadLogs
                .Where(x => x.DispositivoSN == serialNumber)
                .MaxAsync(x => (DateTime?)x.FechaHora);
        }

        public async Task<bool> HasHistoryAsync(string serialNumber)
        {
            return await _context.ActividadLogs.AnyAsync(x => x.DispositivoSN == serialNumber);
        }
    }
}
