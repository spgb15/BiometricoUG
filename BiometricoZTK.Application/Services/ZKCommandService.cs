using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Domain.Interfaces;

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
                string commandId = DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                return $"C:{commandId}:DATA QUERY ATTLOG StartTime={DateTime.Now.AddDays(-1):yyyy-MM-dd HH:mm:ss}";
            }

            if ((DateTime.Now - ultimaFecha.Value).TotalHours > 1)
            {
                var desde = ultimaFecha.Value.AddSeconds(1);
                string commandId = DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                return $"C:{commandId}:DATA QUERY ATTLOG StartTime={desde:yyyy-MM-dd HH:mm:ss}";
            }

            return "OK";
            // return "C:999:DATA DELETE ATTLOG";
        }
    }
}
