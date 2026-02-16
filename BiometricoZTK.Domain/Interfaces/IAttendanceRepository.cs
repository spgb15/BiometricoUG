using BiometricoZTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Domain.Interfaces
{
    public interface IAttendanceRepository
    {
        Task AddRangeAsync(IEnumerable<ActividadLog> logs);
        Task<DateTime?> GetLastLogDateAsync(string serialNumber);
        Task<List<ActividadLog>> GetExistingLogAsync(string sn, DateTime inicio, DateTime fin);
        Task<bool> HasHistoryAsync(string serialNumber);

    }
}
