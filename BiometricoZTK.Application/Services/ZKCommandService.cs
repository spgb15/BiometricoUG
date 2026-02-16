using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Services
{
    public class ZKCommandService : IZKCommandService
    {
        private readonly IAttendanceRepository _repository;

        public ZKCommandService(IAttendanceRepository repository)
        {
            _repository = repository;
        }


        public async Task<string> GetPendingCommands(string sn)
        {
            var ultimaFecha = await _repository.GetLastLogDateAsync(sn);

            if (ultimaFecha == null)
            {
                return $"C:1:DATA QUERY ATTLOG StartTime={DateTime.Now.AddDays(-30):yyyy-MM-dd HH:mm:ss}";
            }

            if ((DateTime.Now - ultimaFecha.Value).TotalMinutes > 5)
            {
                var desde = ultimaFecha.Value.AddSeconds(1);
                return $"C:1:DATA QUERY ATTLOG StartTime={desde:yyyy-MM-dd HH:mm:ss}";
            }

            return "OK";
            // return "C:999:DATA DELETE ATTLOG";
        }
    }
}
